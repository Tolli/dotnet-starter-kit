# Be able to run the script from root or ps-scripts folder
$baseDir = (Get-Location).Path
if ($baseDir -like "*\ps-scripts") {
    $baseDir = $baseDir -replace "\\ps-scripts", ""
}

$currentUser = [System.Security.Principal.WindowsIdentity]::GetCurrent()
$currentUserPrincipal = new-object System.Security.Principal.WindowsPrincipal($currentUser)
if (! $currentUserPrincipal.IsInRole([System.Security.Principal.WindowsBuiltInRole]::Administrator))
{
    $newProcess = new-object System.Diagnostics.ProcessStartInfo "PowerShell";
    $newProcess.Arguments = $myInvocation.MyCommand.Definition;
    $newProcess.Verb = "runas";
    [System.Diagnostics.Process]::Start($newProcess) >$null 2>&1;
    exit;
}
	
# Modify hosts file to add entries
$hostsPath = 'C:\Windows\System32\drivers\etc\hosts'
$hostsEntries = @(
    '127.0.0.1 sts.k8s.tbr.localhost',
    '127.0.0.1 api.k8s.tbr.localhost',
    '127.0.0.1 adminportal.k8s.tbr.localhost',
    '127.0.0.1 pfm.k8s.tbr.localhost',
    '127.0.0.1 insightsportal.k8s.tbr.localhost # Only needed if using Insight Factory',
    '127.0.0.1 kafkaui.k8s.tbr.localhost # Only needed if using kafka-ui',
    '127.0.0.1 kafka.k8s.tbr.localhost # Only needed if kafka should be available for access from outside cluster',
    '127.0.0.1 sql.k8s.tbr.localhost # Only needed if sql should be available for access from outside cluster',
    '127.0.0.1 bfm.k8s.tbr.localhost # Only needed if using bfm',
    '127.0.0.1 core.k8s.tbr.localhost # Only needed if core should be available for access from outside cluster directly',
    '127.0.0.1 prometheus.k8s.tbr.localhost',
    '127.0.0.1 grafana.k8s.tbr.localhost',
    '127.0.0.1 alertmanager.k8s.tbr.localhost'
)
