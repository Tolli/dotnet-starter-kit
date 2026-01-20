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

Set-ExecutionPolicy Bypass -Scope Process -Force
[System.Net.ServicePointManager]::SecurityProtocol = [System.Net.ServicePointManager]::SecurityProtocol -bor 3072

iex ((New-Object System.Net.WebClient).DownloadString('https://community.chocolatey.org/install.ps1'))

Install-Module -Name SqlServer

choco install docker-cli /y
choco install kubernetes-cli -y
choco install kubernetes-helm /y

# WSL
# wsl --install
# Write-host "Please restart!"
# #Restart-Computer -Force

# Install sops
# choco install sops -y

# Docker Desktop
# choco install docker-desktop /y

# AWS
# choco install awscli /y
# choco install eksctl /y

# Azure
# choco install azure-cli /y

# GCP
# choco install gcloudsdk /y
# gcloud components install gke-gcloud-auth-plugin

Write-Host -NoNewLine 'Press any key to close window...';
$null = $Host.UI.RawUI.ReadKey('NoEcho,IncludeKeyDown');
