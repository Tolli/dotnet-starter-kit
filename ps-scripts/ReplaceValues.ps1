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

# Replace ingress values in yaml and env files
ReplaceStringValuesInFiles "*.yaml" $defaultIngress $newIngress
ReplaceStringValuesInFiles "*.env" $defaultIngress $newIngress

# Replace namespace values in yaml, env, json, md and ps1 files
ReplaceStringValuesInFiles "*.yaml" $svcPattern $svcReplacement
ReplaceStringValuesInFiles "*.env" $svcPattern $svcReplacement
ReplaceStringValuesInFiles "*.json" $svcPattern $svcReplacement

ReplaceStringValuesInFiles "*.yaml" $monitoringSvcPattern $monitoringSvcReplacement
ReplaceStringValuesInFiles "*.env" $monitoringSvcPattern $monitoringSvcReplacement
ReplaceStringValuesInFiles "*.json" $monitoringSvcPattern $monitoringSvcReplacement

ReplaceStringValuesInFiles "*.ps1" $nsDocPattern $nsDocReplacement
ReplaceStringValuesInFiles "*.ps1" $nsDocPattern2 $nsDocReplacement2
ReplaceStringValuesInFiles "*.ps1" $nsDocPattern3 $nsDocReplacement3
ReplaceStringValuesInFiles "*.ps1" $nsDocPattern $nsDocReplacement
ReplaceStringValuesInFiles "*.ps1" $nsDocPattern5 $nsDocReplacement5

ReplaceStringValuesInFiles "*.md" $nsDocPattern $nsDocReplacement
ReplaceStringValuesInFiles "*.md" $nsDocPattern2 $nsDocReplacement2
ReplaceStringValuesInFiles "*.md" $nsDocPattern3 $nsDocReplacement3
ReplaceStringValuesInFiles "*.md" $nsDocPattern4 $nsDocReplacement4
ReplaceStringValuesInFiles "*.md" $nsDocPattern5 $nsDocReplacement5