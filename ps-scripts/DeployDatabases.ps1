# Description: This is a script that can be used to deploy solution databases.
#              It will create the databases, users and run the scripts to create schema and applications.
#              This script is used by DeploySolution.ps1.
#              You may need to edit at least the following variables especially if using an existing SQL Server Instance
#              $SQLInstanceName
#              $SQLUser
#              $SQLPassword

param(
	$delete
)


# Be able to run the script from root or ps-scripts folder
$baseDir = (Get-Location).Path
if ($baseDir -like "*\ps-scripts") {
    $baseDir = $baseDir -replace "\\ps-scripts", ""
}

$SQLInstanceName = "localhost"
$SQLUser = "sa"
$SQLPassword = "saAdministrator123"
$MenigaUser = "tbr"
$MenigaPassword = "administrator123"

$APP_DATABASE="TBRAppDb"

$DATABASES = @($APP_DATABASE)

if ($null -eq $delete) {
  Write-Output "!!!WARNING!!!"
  Write-Output "SQLInstanceName: $SQLInstanceName"
  Write-Output "The following database will be deleted:"
  Write-Output "- CORE_DATABASES: $DATABASES"
  Write-Output "Are you sure you want to continue? (y/n)"
  $confirmation = Read-Host
  if ($confirmation -ne "y") {
    Write-Output "Exiting..."
    exit
  }
}

. $baseDir\ps-scripts\Scripts.ps1

# Remove-MenigaDatabaseUsers -DBUserId $SQLUser -DBPassword $SQLPassword -InstanceName $SQLInstanceName -MenigaSQLLogin $MenigaUser
Add-MenigaDatabaseUsers -DBUserId $SQLUser -DBPassword $SQLPassword -InstanceName $SQLInstanceName -MenigaSQLLogin $MenigaUser -MenigaSQLLoginPassword $MenigaPassword

if ($deployCore -eq $true) {
    Remove-MenigaDatabases -DBNames $CORE_DATABASES -DBUserId $MenigaUser -DBPassword $MenigaPassword -InstanceName $SQLInstanceName -DeleteForSure 1
    Remove-MenigaDatabases -DBNames $STS_DATABASES -DBUserId $MenigaUser -DBPassword $MenigaPassword -InstanceName $SQLInstanceName -DeleteForSure 1
    New-MenigaDatabases -DBNames $CORE_DATABASES -DBUserId $MenigaUser -DBPassword $MenigaPassword -InstanceName $SQLInstanceName
    New-MenigaDatabases -DBNames $STS_DATABASES -DBUserId $MenigaUser -DBPassword $MenigaPassword -InstanceName $SQLInstanceName

    # Core App Db
    Invoke-Database-Scripts -DBName $CORE_APP_DATABASE -DBUserId $MenigaUser -DBPassword $MenigaPassword -InstanceName $SQLInstanceName -FilesPath $baseDir\database\scripts\meniga-hub\Schema
    Invoke-Database-Scripts -DBName $CORE_APP_DATABASE -DBUserId $MenigaUser -DBPassword $MenigaPassword -InstanceName $SQLInstanceName -FilesPath $baseDir\database\scripts\meniga-hub\VanillaData

    # Core Merchant Db
    Invoke-Database-Scripts -DBName $CORE_MERCHANT_DATABASE -DBUserId $MenigaUser -DBPassword $MenigaPassword -InstanceName $SQLInstanceName -FilesPath $baseDir\database\scripts\meniga-hub\Schema
    Invoke-Database-Scripts -DBName $CORE_MERCHANT_DATABASE -DBUserId $MenigaUser -DBPassword $MenigaPassword -InstanceName $SQLInstanceName -FilesPath $baseDir\database\scripts\meniga-hub\VanillaData

    # Core User DB's
    Invoke-Database-Scripts -DBName $CORE_USER1_DATABASE -DBUserId $MenigaUser -DBPassword $MenigaPassword -InstanceName $SQLInstanceName -FilesPath $baseDir\database\scripts\meniga-hub\Schema
    Invoke-Database-Scripts -DBName $CORE_USER2_DATABASE -DBUserId $MenigaUser -DBPassword $MenigaPassword -InstanceName $SQLInstanceName -FilesPath $baseDir\database\scripts\meniga-hub\Schema

    # STS Db
    Invoke-Database-Scripts -DBName $STS_DATABASE -DBUserId $MenigaUser -DBPassword $MenigaPassword -InstanceName $SQLInstanceName -FilesPath $baseDir\database\scripts\meniga-hub\STS
}

if($deployCA -eq $true) {
    Remove-MenigaDatabases -DBNames $CA_DATABASES -DBUserId $MenigaUser -DBPassword $MenigaPassword -InstanceName $SQLInstanceName -DeleteForSure 1
    New-MenigaDatabases -DBNames $CA_DATABASES -DBUserId $MenigaUser -DBPassword $MenigaPassword -InstanceName $SQLInstanceName

    # Cashflow Assistant Meniga App DB
    Invoke-Database-Scripts -DBName $CORE_APP_DATABASE -DBUserId $MenigaUser -DBPassword $MenigaPassword -InstanceName $SQLInstanceName -FilesPath $baseDir\database\scripts\cashflow-assistant\MenigaAppDb

    # Cashflow Assistant Meniga User DB's
    Invoke-Database-Scripts -DBName $CORE_USER1_DATABASE -DBUserId $MenigaUser -DBPassword $MenigaPassword -InstanceName $SQLInstanceName -FilesPath $baseDir\database\scripts\cashflow-assistant\MenigaUserDb
    Invoke-Database-Scripts -DBName $CORE_USER2_DATABASE -DBUserId $MenigaUser -DBPassword $MenigaPassword -InstanceName $SQLInstanceName -FilesPath $baseDir\database\scripts\cashflow-assistant\MenigaUserDb

    # Cashflow Assistant User DB's
    Invoke-Database-Scripts -DBName $CA_USER1_DATABASE -DBUserId $MenigaUser -DBPassword $MenigaPassword -InstanceName $SQLInstanceName -FilesPath $baseDir\database\scripts\cashflow-assistant\CashflowAssistantUserDb
    Invoke-Database-Scripts -DBName $CA_USER2_DATABASE -DBUserId $MenigaUser -DBPassword $MenigaPassword -InstanceName $SQLInstanceName -FilesPath $baseDir\database\scripts\cashflow-assistant\CashflowAssistantUserDb
}

if ($deployIF -eq $true) {
    Remove-MenigaDatabases -DBNames $IF_DATABASES -DBUserId $MenigaUser -DBPassword $MenigaPassword -InstanceName $SQLInstanceName -DeleteForSure 1
    New-MenigaDatabases -DBNames $IF_DATABASES -DBUserId $MenigaUser -DBPassword $MenigaPassword -InstanceName $SQLInstanceName

    #Insight Factory Meniga App DB - optional script if message-dispatcher-notification-framework is deployed
    Invoke-Database-Scripts -DBName $CORE_APP_DATABASE -DBUserId $MenigaUser -DBPassword $MenigaPassword -InstanceName $SQLInstanceName -FilesPath $baseDir\database\scripts\insights-factory\message-dispatcher-notification-framework\meniga-app

    # Insights Factory DB
    Invoke-Database-Scripts -DBName $IF_DATABASE -DBUserId $MenigaUser -DBPassword $MenigaPassword -InstanceName $SQLInstanceName -FilesPath $baseDir\database\scripts\insights-factory\InsightFactoryApp

    # Insight Factory IF, FeedApi and MessageDispatcher User1**
    Invoke-Database-Scripts -DBName $IF_USER1_DATABASE -DBUserId $MenigaUser -DBPassword $MenigaPassword -InstanceName $SQLInstanceName -FilesPath $baseDir\database\scripts\insights-factory\InsightFactoryUser
    
    # Insight Factory IF, FeedApi and MessageDispatcher User2**
    Invoke-Database-Scripts -DBName $IF_USER2_DATABASE -DBUserId $MenigaUser -DBPassword $MenigaPassword -InstanceName $SQLInstanceName -FilesPath $baseDir\database\scripts\insights-factory\InsightFactoryUser

    # metadata - IF
    Invoke-Database-Scripts -DBName $IF_DATABASE -DBUserId $MenigaUser -DBPassword $MenigaPassword -InstanceName $SQLInstanceName -FilesPath $baseDir\database\scripts\insights-factory\InsightFactoryMetadata

    if($deploySMR -eq $true) {
        Remove-MenigaDatabases -DBNames $SMR_DATABASES -DBUserId $MenigaUser -DBPassword $MenigaPassword -InstanceName $SQLInstanceName -DeleteForSure 1
        New-MenigaDatabases -DBNames $SMR_DATABASES -DBUserId $MenigaUser -DBPassword $MenigaPassword -InstanceName $SQLInstanceName
        
        # Smart Money Rules User DB's
        Invoke-Database-Scripts -DBName $SMR_USER1_DATABASE -DBUserId $MenigaUser -DBPassword $MenigaPassword -InstanceName $SQLInstanceName -FilesPath $baseDir\database\scripts\smart-money-rules\UserDbSqlScriptsFromScratch
        Invoke-Database-Scripts -DBName $SMR_USER2_DATABASE -DBUserId $MenigaUser -DBPassword $MenigaPassword -InstanceName $SQLInstanceName -FilesPath $baseDir\database\scripts\smart-money-rules\UserDbSqlScriptsFromScratch
    }
    
}
