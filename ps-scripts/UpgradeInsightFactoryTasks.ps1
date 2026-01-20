# Ensure psyml module (v1.0.0) is installed and imported
$moduleName = 'psyml'
$moduleVersion = '1.0.0'

if (-not (Get-Module -ListAvailable $moduleName | Where-Object { $_.Version -eq $moduleVersion })) {
    try {
        Install-Module $moduleName -RequiredVersion $moduleVersion -Scope CurrentUser -Force -AllowClobber
    } catch {
        Write-Error "Failed to install module '$moduleName' version '$moduleVersion'. Error: $_"
        throw
    }
}

Import-Module $moduleName -RequiredVersion $moduleVersion -Force

Function Update-InsightsFactoryTags {
    <#
    .SYNOPSIS
    Updates InsightFactory helm values file based on InsightFactory artifacts manifest

    .PARAMETER insightsFactoryValuesYamlFile
    InsightsFactory helm values file

    .PARAMETER insightFactoryArtifactsYamlFile
    InsightsFactory artifact manifest file
    #>
    param(
        [string]$insightsFactoryValuesYamlFile,
        [string]$insightFactoryArtifactsYamlFile
    )

        # Mappings between InsightsFactory helm chart and InsightsFactory artifacts manifest
    $mappings = @{
        'digitalBankingPlugin'        = 'meniga-digital-banking-plugin'
        'usersProviderPlugin'         = 'meniga-digital-banking-users-provider'
        'cashflowAssistantPlugin'     = 'cashflow-assistant-plugin'
        'insightsPortal'              = 'mono-admin'
        'backgroundJobsScheduler'     = 'background-jobs-scheduler'
        'bankAdmin'                   = 'bank-Admin'
        'externalEventsConsumer'      = 'external-events-consumer'
        'feedApi'                     = 'feed-api'
        'feedApiBackend'              = 'feed-api'
        'insightDefinitionsScheduler' = 'insight-definitions-scheduler'
        'messageDispatcherKafka'      = 'message-dispatcher-kafka'
        'triggeringEngine'            = 'triggering-engine'
    }

    # Read and parse the insightsFactoryValuesYaml file
    $insightsFactoryValuesContent = Get-Content -Path $insightsFactoryValuesYamlFile -Raw
    $insightsFactoryValuesYaml = ConvertFrom-Yaml $insightsFactoryValuesContent

    # Read and parse the artifacts YAML file
    $artifactsContent = Get-Content -Path $insightFactoryArtifactsYamlFile -Raw
    $artifactsYaml = ConvertFrom-Yaml $artifactsContent

    # Function to recursively find and update 'tag' properties
    function Update-TagProperty {
        param(
            [psobject]$Object,
            [string]$NewTag
        )

        foreach ($property in $Object.PSObject.Properties) {
            if ($property.Name -eq 'tag') {
                # Update the tag value
                Write-Verbose "Updating $Object -> $NewTag"
                $Object.$($property.Name) = $NewTag
            }
            elseif ($property.Value -is [psobject]) {
                # Recurse into nested objects
                Update-TagProperty -Object $property.Value -NewTag $NewTag
            }
            elseif ($property.Value -is [System.Collections.IEnumerable] -and
                -not ($property.Value -is [string])) {
                # Recurse into arrays or collections
                foreach ($item in $property.Value) {
                    if ($item -is [psobject]) {
                        Update-TagProperty -Object $item -NewTag $NewTag
                    }
                }
            }
        }
    }

    foreach ($ifKey in $mappings.Keys) {
        Write-Host "Updating tag for $ifKey"
        $artifactKey = $mappings[$ifKey]

        # Get the tag from the artifacts YAML
        $artifactEntry = $artifactsYaml.artifacts.$artifactKey

        if ($artifactEntry -ne $null) {
            $fullTag = $artifactEntry.tag
            if ($fullTag -ne $null) {
                # Extract the image tag
                $imageTag = $fullTag.Split(":")[-1]

                # Ensure the key exists in insightsFactoryValuesYaml
                $ifYamlSection = $insightsFactoryValuesYaml.$ifKey
                if ($ifYamlSection -ne $null) {
                    # Update 'tag' properties under this section
                    Update-TagProperty -Object $ifYamlSection -NewTag $imageTag
                }
                else {
                    Write-Verbose "Key '$ifKey' does not exist in insightsFactoryValuesYaml. Skipping."
                }
            }
            else {
                Write-Warning "Tag not found for artifact key '$artifactKey'."
            }
        }
        else {
            Write-Warning "Artifact key '$artifactKey' not found in artifacts file."
        }
    }

    # Convert back to YAML and write to file
    $newYamlContent = ConvertTo-Yaml -Data $insightsFactoryValuesYaml
    Set-Content -Path $insightsFactoryValuesYamlFile -Value $newYamlContent
}

Function Update-InsightsFactoryDatabaseScripts {
    <#
    .SYNOPSIS
    Updates Insight Factory database scripts based on the provided release directory

    .PARAMETER insightsFactoryReleaseDir
    Path to Insight Factory release directory

    .PARAMETER insightFactoryAppDatabaseDirectory
    Path to InsightsFactory App database directory

    .PARAMETER insightFactoryUserDatabaseDirectory
    Path to InsightsFactory User database directory

    .PARAMETER insightFactoryMetadataDatabaseDirectory
    Path to InsightsFactory Metadata scripts directory
    #>
    param(
        [string]$insightsFactoryReleaseDir,
        [string]$insightFactoryAppDatabaseDirectory,
        [string]$insightFactoryUserDatabaseDirectory,
        [string]$insightFactoryMetadataDatabaseDirectory
    )

    # Setup of Insight Factory databases using split into single InsightFactoryApp database and N InsightFactoryUser databases
    #  If needed Insight Factory databases can be split more granuraly resulting in database per each schema
    #  insightsfactory_usr schema can be also sharded indepndently from the Meniga PFM User databases
    $mappings = @{
        'scripts\bank-admin\insightsfactory\from-scratch'                                       = $insightFactoryAppDatabaseDirectory
        'scripts\insight-definitions-scheduler\insightsfactory_scheduler\from-scratch'          = $insightFactoryAppDatabaseDirectory
        'scripts\triggering-engine\sharding\from-scratch'                                       = $insightFactoryAppDatabaseDirectory

        'scripts\bank-admin\metrics\from-scratch'                                               = $insightFactoryUserDatabaseDirectory
        'scripts\feed-api\feed\from-scratch'                                                    = $insightFactoryUserDatabaseDirectory
        'scripts\message-dispatcher\dispatcher\from-scratch'                                    = $insightFactoryUserDatabaseDirectory
        'scripts\triggering-engine\insightsfactory_usr\from-scratch'                            = $insightFactoryUserDatabaseDirectory

        'scripts\bank-admin\insightsfactory\metadata\truncate'                                  = $insightFactoryMetadataDatabaseDirectory
        'scripts\bank-admin\insightsfactory\metadata\digital-banking-events'                    = $insightFactoryMetadataDatabaseDirectory
        'scripts\bank-admin\insightsfactory\metadata\digital-banking'                           = $insightFactoryMetadataDatabaseDirectory
        'scripts\bank-admin\insightsfactory\metadata\cashflow-assistant'                        = $insightFactoryMetadataDatabaseDirectory
    }

    # Clean database directories passed in as arguments
    $dbDirs = @($insightFactoryAppDatabaseDirectory, $insightFactoryUserDatabaseDirectory, $insightFactoryMetadataDatabaseDirectory)
    foreach ($dir in $dbDirs) {
        if (Test-Path $dir) {
            Write-Host "Cleaning directory: $dir"
            Get-ChildItem -Path $dir -Recurse -Force | Remove-Item -Force -Recurse
        } else {
            Write-Host "Creating directory: $dir"
            New-Item -ItemType Directory -Path $dir | Out-Null
        }
    }

    # For each entry in the mappings - using insightsFactoryReleaseDir as base path move all the files from the mapping key to the target directory passed in as value
    foreach ($sourceRelPath in $mappings.Keys) {
        $targetDir = $mappings[$sourceRelPath]
        $sourcePath = Join-Path $insightsFactoryReleaseDir $sourceRelPath
        if (Test-Path $sourcePath) {
            $files = Get-ChildItem -Path $sourcePath -File -Recurse -ErrorAction SilentlyContinue
            if ($files.Count -eq 0) {
                Write-Host "  No files found in $sourcePath."
            }
            foreach ($file in $files) {
                $destPath = Join-Path $targetDir $file.Name
                Write-Host "  Copying $($file.FullName) to $destPath"
                Copy-Item -Path $file.FullName -Destination $destPath -Force
            }
        } else {
            Write-Warning "Source path '$sourcePath' does not exist. Skipping."
        }
    }
}