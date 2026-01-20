# Description: This is a script that is used to deploy Meniga Merchant Directory.
# It will create the a staging table and insert merchant data into it and then use that to populate the merchant directory.

# Be able to run the script from root or ps-scripts folder
$baseDir = (Get-Location).Path
if ($baseDir -like "*\ps-scripts") {
    $baseDir = $baseDir -replace "\\ps-scripts", ""
}

$SQLInstanceName = "localhost"
$SQLUser = "sa"
$SQLPassword = "rootAdmin321"

$MERCHANT_DATABASE = "Core"

. $baseDir\ps-scripts\MenigaScripts.ps1

# Core Db
Invoke-Single-Script -DBName $MERCHANT_DATABASE -DBUserId $SQLUser -DBPassword $SQLPassword -InstanceName $SQLInstanceName -FilePath $baseDir\database\scripts\merchant-directory\0001-CreateMerchantsStagingTable.sql
Invoke-Single-Script -DBName $MERCHANT_DATABASE -DBUserId $SQLUser -DBPassword $SQLPassword -InstanceName $SQLInstanceName -FilePath $baseDir\database\scripts\merchant-directory\0002-MMSStagingInsert.sql
Invoke-Single-Script -DBName $MERCHANT_DATABASE -DBUserId $SQLUser -DBPassword $SQLPassword -InstanceName $SQLInstanceName -FilePath $baseDir\database\scripts\merchant-directory\0003-MerchantsStagingIntegrityChecks.sql
Invoke-Single-Script -DBName $MERCHANT_DATABASE -DBUserId $SQLUser -DBPassword $SQLPassword -InstanceName $SQLInstanceName -FilePath $baseDir\database\scripts\merchant-directory\0004-MMSDirectoryMerge.sql
Invoke-Single-Script -DBName $MERCHANT_DATABASE -DBUserId $SQLUser -DBPassword $SQLPassword -InstanceName $SQLInstanceName -FilePath $baseDir\database\scripts\merchant-directory\0005-MDHealthCheck.sql
Invoke-Single-Script -DBName $MERCHANT_DATABASE -DBUserId $SQLUser -DBPassword $SQLPassword -InstanceName $SQLInstanceName -FilePath $baseDir\database\scripts\merchant-directory\0006-MerchantMapping.sql
Invoke-Single-Script -DBName $MERCHANT_DATABASE -DBUserId $SQLUser -DBPassword $SQLPassword -InstanceName $SQLInstanceName -FilePath $baseDir\database\scripts\merchant-directory\0007-DynamicAttributes.sql
