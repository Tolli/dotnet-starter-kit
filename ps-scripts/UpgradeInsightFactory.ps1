param(
    [Parameter(Mandatory = $true)]
    [ValidateNotNullOrEmpty()]
    [string]$insightsFactoryVersion,

    [Parameter(Mandatory = $true)]
    [ValidateNotNullOrEmpty()]
    [string]$insightsFactoryReleaseDir
)

if ([string]::IsNullOrWhiteSpace($insightsFactoryVersion)) {
    Write-Output "Insight Factory version needs to be provided and cannot be empty."
    throw "Insight Factory version needs to be provided and cannot be empty."
}

if ([string]::IsNullOrWhiteSpace($insightsFactoryReleaseDir)) {
    Write-Output "Insight Factory release directory needs to be provided and cannot be empty."
    throw "Insight Factory release directory needs to be provided and cannot be empty."
}


$baseDir = (Get-Location).Path
if ($baseDir -like "*\ps-scripts") {
    $baseDir = $baseDir -replace "\\ps-scripts", ""
}

. $baseDir\ps-scripts\UpgradeInsightFactoryTasks.ps1

Update-InsightsFactoryDatabaseScripts -insightsFactoryReleaseDir $insightsFactoryReleaseDir `
    -insightFactoryAppDatabaseDirectory $baseDir\database\scripts\insights-factory\InsightFactoryApp\Create `
    -insightFactoryUserDatabaseDirectory $baseDir\database\scripts\insights-factory\InsightFactoryUser\Create `
    -insightFactoryMetadataDatabaseDirectory $baseDir\database\scripts\insights-factory\InsightFactoryMetadata\Upgrade

Update-InsightsFactoryTags -insightFactoryArtifactsYamlFile $insightsFactoryReleaseDir\manifests\insight-factory.$insightsFactoryVersion.yml `
    -insightsFactoryValuesYamlFile $baseDir\meniga\insights-factory\insights-factory-values-obs.yaml `

Update-InsightsFactoryTags -insightFactoryArtifactsYamlFile $insightsFactoryReleaseDir\manifests\insight-factory.$insightsFactoryVersion.yml `
    -insightsFactoryValuesYamlFile $baseDir\meniga\insights-factory\insights-factory-values-security.yaml `

Update-InsightsFactoryTags -insightFactoryArtifactsYamlFile $insightsFactoryReleaseDir\manifests\insight-factory.$insightsFactoryVersion.yml `
    -insightsFactoryValuesYamlFile $baseDir\meniga\insights-factory\insights-factory-values.yaml `
