Write-Output "!!!WARNING!!!"
Write-Output "This script needs to be run in Powershell 7 or later, if running in earlier versions files will get corrupted"
Write-Output "This script will replace values in all files based on patterns defined in the script"
Write-Output "Are you sure you want to continue? (y/n)"
$confirmation = Read-Host
if ($confirmation -ne "y") {
    Write-Output "Exiting..."
    exit
}

# Be able to run the script from root or ps-scripts folder
$baseDir = (Get-Location).Path
if ($baseDir -like "*\ps-scripts") {
    $baseDir = $baseDir -replace "\\ps-scripts", ""
}

. $baseDir\ps-scripts\MenigaScripts.ps1

$defaultNamespace = "meniga-getting-started"
$defaultMonitoringNamespace = "$defaultNamespace-monitoring"
$defaultIngress = "k8s.tbr.localhost"

$newNamespace = "sab"
$newIngress = "sab.meniga.localhost"
$newMonitoringNamespace = "$newNameSpace-monitoring"

$svcPattern = "$defaultNamespace.svc"
$svcReplacement =  "$newNamespace.svc"

$monitoringSvcPattern = "$defaultMonitoringNamespace.svc"
$monitoringSvcReplacement =  "$newMonitoringNamespace.svc"

$nsDocPattern = "``$defaultNamespace``"
$nsDocReplacement = "``$newNamespace``"

$nsDocPattern2 = " $defaultNamespace"
$nsDocReplacement2 = " $newNamespace"

$nsDocPattern3 = """$defaultNamespace"""
$nsDocReplacement3 = """$newNamespace"""

$nsDocPattern4 = "--namespace=$defaultNamespace"
$nsDocReplacement4 = "--namespace=$newNamespace"

$nsDocPattern5 = """$defaultMonitoringNamespace"""
$nsDocReplacement5 = """$newMonitoringNamespace"""

# Revert the changes, comment out the following lines if you want to keep the changes
# Replace ingress values in yaml and env files
ReplaceStringValuesInFiles "*.yaml" $newIngress $defaultIngress
ReplaceStringValuesInFiles "*.env" $newIngress $defaultIngress

# Replace namespace values in yaml, env, json, md and ps1 files
ReplaceStringValuesInFiles "*.yaml" $svcReplacement $svcPattern
ReplaceStringValuesInFiles "*.env" $svcReplacement $svcPattern
ReplaceStringValuesInFiles "*.json" $svcReplacement $svcPattern

ReplaceStringValuesInFiles "*.yaml" $monitoringSvcReplacement $monitoringSvcPattern
ReplaceStringValuesInFiles "*.env" $monitoringSvcReplacement $monitoringSvcPattern
ReplaceStringValuesInFiles "*.json" $monitoringSvcReplacement $monitoringSvcPattern

ReplaceStringValuesInFiles "*.ps1" $nsDocReplacement $nsDocPattern
ReplaceStringValuesInFiles "*.ps1" $nsDocReplacement2 $nsDocPattern2
ReplaceStringValuesInFiles "*.ps1" $nsDocReplacement3 $nsDocPattern3
ReplaceStringValuesInFiles "*.ps1" $nsDocReplacement $nsDocPattern
ReplaceStringValuesInFiles "*.ps1" $nsDocReplacement5 $nsDocPattern5

ReplaceStringValuesInFiles "*.md" $nsDocReplacement $nsDocPattern
ReplaceStringValuesInFiles "*.md" $nsDocReplacement2 $nsDocPattern2
ReplaceStringValuesInFiles "*.md" $nsDocReplacement3 $nsDocPattern3
ReplaceStringValuesInFiles "*.md" $nsDocReplacement4 $nsDocPattern4
ReplaceStringValuesInFiles "*.md" $nsDocReplacement5 $nsDocPattern5