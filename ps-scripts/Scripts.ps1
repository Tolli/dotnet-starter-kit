# Description: This files includes database functions.
#              These functions are used by DeployDatabases.ps1.

# Exit immediately if a command exits with a non-zero status
$ErrorActionPreference = "Stop"

function New-Databases {
    [CmdletBinding()]
    param(
        [array]$DBNames,
        [string]$DBUserId,
        [string]$DBPassword,
        [string]$InstanceName
    )

    if (-not $DBNames) {
        Write-Host "No database names provided, exiting..."
        return
    }

    foreach ($db in $DBNames) {
        Write-Host "[create_databases.ps1] Creating database $db"
        
        Invoke-Sqlcmd -ServerInstance $InstanceName -Username $DBUserId -Password $DBPassword -Query "IF DB_ID (N'$db') IS NULL CREATE DATABASE $db;" -TrustServerCertificate
    }
}    

function Remove-Databases {
    [CmdletBinding()]
    param(
        [array]$DBNames,
        [string]$DBUserId,
        [string]$DBPassword,
        [string]$InstanceName,
        [bool]$DeleteForSure
    )

    if (-not $DBNames) {
        Write-Host "No database names provided, exiting..."
        return
    }

    foreach ($db in $DBNames) {
        Write-Host "Deleting database $db"
        Invoke-Sqlcmd -ServerInstance $InstanceName -Username $DBUserId -Password $DBPassword -Query "IF 'True'='${DeleteForSure}' AND EXISTS (SELECT 1 FROM sys.databases WHERE [name] = N'${db}') BEGIN ALTER DATABASE ${db} SET SINGLE_USER WITH ROLLBACK IMMEDIATE;DROP DATABASE ${db};END" -TrustServerCertificate
    }
}    


function Add-DatabaseUsers {
    [CmdletBinding()]
    param(
        [string]$DBUserId,
        [string]$DBPassword,
        [string]$InstanceName,
        [string]$UserSQLLogin,
        [string]$UserSQLLoginPassword

    )

    if (-not $DbUserId) {
        Write-Host "No DBUserId provided, exiting..."
        return
    }

    Write-Host "[Add-DatabaseUsers] Creating user $UserSQLLogin"
    
    Invoke-Sqlcmd -ServerInstance $InstanceName -Username $DBUserId -Password $DBPassword -Query "IF NOT EXISTS (SELECT name FROM master.sys.server_principals WHERE name = '$UserSQLLogin') BEGIN CREATE LOGIN $UserSQLLogin WITH PASSWORD = '$UserSQLLoginPassword'; ALTER SERVER ROLE dbcreator ADD MEMBER $UserSQLLogin END" -TrustServerCertificate
}

function Remove-DatabaseUsers {
    [CmdletBinding()]
    param(
        [string]$DBUserId,
        [string]$DBPassword,
        [string]$InstanceName,
        [string]$UserSQLLogin

    )

    if (-not $DbUserId) {
        Write-Host "No DBUserId provided, exiting..."
        return
    }

    Write-Host "[Remove-DatabaseUsers] Deleting user $UserSQLLogin"
    
    Invoke-Sqlcmd -ServerInstance $InstanceName -Username $DBUserId -Password $DBPassword -Query "IF EXISTS (SELECT name FROM master.sys.server_principals WHERE name = '$UserSQLLogin') BEGIN DROP LOGIN $UserSQLLogin END" -TrustServerCertificate
}


function Invoke-Database-Scripts {
    [CmdletBinding()]
    param(
        [string]$DBName,
        [string]$DBUserId,
        [string]$DBPassword,
        [string]$InstanceName,
        [string]$FilesPath
    )

    Write-Host "[Invoke-Database-Scripts] processing files at '$($FilesPath)' on $DBName database"

    if (-not $DBName) {
        Write-Host "No database name provided, exiting..."
        return
    }

    # Get all directories in the specified path
    $directories = Get-ChildItem -Path $FilesPath -Directory

    foreach ($directory in $directories) {
        Write-Host "[Invoke-Database-Scripts] processing directory '$($directory.FullName)'"
        
        # Get all .sql files in the current directory
        $sqlFiles = Get-ChildItem -Path $directory.FullName -Filter *.sql

        foreach ($file in $sqlFiles) {
            Write-Host "[Invoke-Database-Scripts] processing file '$($file.FullName)'"
            
            # Execute the SQL script using sqlcmd
            Invoke-Sqlcmd -ServerInstance $InstanceName -Username $DBUserId -Password $DBPassword -Database $DBName -InputFile $file.FullName -TrustServerCertificate
            # sqlcmd -C -S $InstanceName -U $DBUserId -P $DBPassword -d $DBName -i $file.FullName -I
        }
    }
}

function Invoke-Single-Script {
    [CmdletBinding()]
    param(
        [string]$DBName,
        [string]$DBUserId,
        [string]$DBPassword,
        [string]$InstanceName,
        [string]$FilePath
    )

    if (-not $DBName) {
        Write-Host "No database name provided, exiting..."
        return
    }

    
    # Get all .sql files in the current directory
    $file = Get-Item -Path $FilePath

    Write-Host "[Invoke-Database-Scripts] processing file '$($file.FullName)'"
    
    # Execute the SQL script using sqlcmd
    Invoke-Sqlcmd -ServerInstance $InstanceName -Username $DBUserId -Password $DBPassword -Database $DBName -InputFile $file.FullName -TrustServerCertificate
    # sqlcmd -C -S $InstanceName -U $DBUserId -P $DBPassword -d $DBName -i $file.FullName -I
}

function ReplaceStringValuesInFiles {
    [CmdletBinding()]
    param(
        [string]$filter, 
        [string]$oldValue, 
        [string]$newValue
    )

    # Be able to run the script from root or ps-scripts folder
    $baseDir = (Get-Location).Path
    if ($baseDir -like "*\ps-scripts") {
        $baseDir = $baseDir -replace "\\ps-scripts", ""
    }

    $configFiles = Get-ChildItem $baseDir $filter -rec
    foreach ($file in $configFiles)
    {
        if($file.PSPath -like "*\ReplaceValues*" -or $file.PSPath -like "*\RevertValues*") {
            continue
        }

        (Get-Content -Raw $file.PSPath)  |
        Foreach-Object { $_ -replace $oldValue, $newValue } |
        Set-Content -NoNewLine -Encoding UTF8 $file.PSPath
    }
}