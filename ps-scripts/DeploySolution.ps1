# Description: This is a script that can be used to automatically run all necessary steps to setup Meniga solution in this repo.
#              It will tear down and deploy to the Kubernetes cluster in context.
#              Make sure you have gone through steps 1.-5. in README.md:
#              1. Get Docker Hub account
#              2. Get access for your Docker Hub account to Meniga repository in Docker Hub from your Meniga representative
#              3. Install tools
#              4. Configure hostnames
#              5. Setup platform
#              You may need to edit at least the following variables in DeployMenigaDatabases.ps1 especially if using an existing SQL Server Instance
#              $SQLInstanceName
#              $SQLUser
#              $SQLPassword

param(
    $namespace,
    $deployCA,
    $deployIF,
    $deploySMR
)

# Be able to run the script from root or ps-scripts folder
$baseDir = (Get-Location).Path
if ($baseDir -like "*\ps-scripts") {
    $baseDir = $baseDir -replace "\\ps-scripts", ""
}

# deploy CA and IF by default
if ($null -eq $deployCA) {
    $deployCA = $true
}

if ($null -eq $deployIF) {
    $deployIF = $true
}

if ($null -eq $deploySMR) {
    $deploySMR = $true
}

if ($null -eq $namespace) {
    $namespace = "meniga-getting-started"
}

Write-Output "!!!WARNING!!!"
Write-Output "Make sure you have gone through steps 1.-5. in README.md"
Write-Output "1. Get Docker Hub account"
Write-Output "2. Get access for your Docker Hub account to Meniga repository in Docker Hub from your Meniga representative"
Write-Output "3. Install tools"
Write-Output "4. Configure hostnames"
Write-Output "5. Setup platform"
Write-Output "Current context is:"
kubectl config current-context
Write-Output "Current namespace is:"
Write-Output $namespace
Write-Output "Make sure SQLInstanceName, SQLUser and SQLPassword variables in DeployMenigaDatabases.ps1 are correct especially if using an existing SQL Server Instance"
Write-Output "This script will delete your existing namespace in current context, databases and recreate everything"
Write-Output "Are you sure you want to continue? (y/n)"
$confirmation = Read-Host
if ($confirmation -ne "y") {
    Write-Output "Exiting..."
    exit
}

Write-Output "Docker Hub credentials with access to Meniga repository in Docker Hub"
$username = Read-Host 'Username'
$password = Read-Host 'Password' -AsSecureString

# Login to Docker Hub
Write-Output "Logging in to Docker Hub..."
$marshal = [Runtime.InteropServices.Marshal]
$pointer = $marshal::SecureStringToBSTR($password)
$plainPassword = $marshal::PtrToStringAuto($pointer)
$plainPassword | helm registry login registry-1.docker.io -u $username --password-stdin

# Delete namespace if it exists
$namespaceExists = kubectl get ns | Select-String $namespace
if ($namespaceExists) {
   Write-Output "Deleting existing namespace $namespace..."
   kubectl delete namespace $namespace
}

kubectl wait ns/$namespace --for='delete'

# Create namespace
Write-Output "Creating namespace $namespace..."
kubectl create namespace $namespace
kubectl config set-context --current --namespace=$namespace

Write-Output "Deploying Infrastructure..."
. $baseDir\ps-scripts\DeployInfrastructure.ps1 -namespace $namespace -dockerUserName $username -dockerPassword $password

Write-Output "Deploying Meniga databases..."
. $baseDir\ps-scripts\DeployMenigaDatabases.ps1 -deployCore $true -deployCA $deployCA -deployIF $deployIF -deploySMR $deploySMR -delete $True

Write-Output "Deploying Meniga applications..."
. $baseDir\ps-scripts\DeployMenigaApplications.ps1 -deployCA $deployCA -deployIF $deployIF -deploySMR $deploySMR -namespace $namespace
