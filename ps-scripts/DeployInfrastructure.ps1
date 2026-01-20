# Description: This is a script that can be used to automatically run all necessary steps to setup the infrastructure for the solution in this repo.
#              It will tear down and deploy to the Kubernetes cluster in context.
#              You may need to edit at least the following variables in DeployDatabases.ps1 especially if using an existing SQL Server Instance
#              $SQLInstanceName
#              $SQLUser
#              $SQLPassword

param(
    $namespace,
    $dockerUsername,
    [SecureString] $dockerPassword,
    $deployKafka
)

# Be able to run the script from root or ps-scripts folder
$baseDir = (Get-Location).Path
if ($baseDir -like "*\ps-scripts") {
    $baseDir = $baseDir -replace "\\ps-scripts", ""
}

if ($null -eq $namespace) {
    $namespace = "tbr"
}

if ($null -eq $dockerUsername) {
    Write-Output "Docker Hub credentials with access to Meniga repository in Docker Hub"
    $dockerUserName = Read-Host 'Username'
    $dockerPassword = Read-Host 'Password' -AsSecureString

    # Login to Docker Hub
    Write-Output "Logging in to Docker Hub..."
    $marshal = [Runtime.InteropServices.Marshal]
    $pointer = $marshal::SecureStringToBSTR($dockerPassword)
    $plainPassword = $marshal::PtrToStringAuto($pointer)
    #$plainPassword = [System.Runtime.InteropServices.Marshal]::PtrToStringAuto([System.Runtime.InteropServices.Marshal]::SecureStringToBSTR($password))
    $plainPassword | helm registry login registry-1.docker.io -u $dockerUserName --password-stdin
} else {
    $plainPassword = [System.Runtime.InteropServices.Marshal]::PtrToStringAuto([System.Runtime.InteropServices.Marshal]::SecureStringToBSTR($password))
}

$namespaceExists = kubectl get ns | Select-String $namespace
if ($namespaceExists) {
    Write-Output "namespace $namespace exists, deploying to it..."
}
else {
    # Create namespace
    Write-Output "Creating namespace $namespace..."
    kubectl create namespace $namespace
}

kubectl config set-context --current --namespace=$namespace


# Deploy nginx controller and wait for it to be ready
Write-Output "Deploying NGINX Ingress controller..."
kubectl apply -f https://raw.githubusercontent.com/kubernetes/ingress-nginx/controller-v1.12.1/deploy/static/provider/cloud/deploy.yaml
kubectl apply -f $baseDir/infrastructure/nginx/disable-https-redirect.yaml
kubectl wait --namespace ingress-nginx --for=condition=available --timeout=600s deployment/ingress-nginx-controller

# Create Kubernetes secret for Docker registry
Write-Output "Creating Docker registry secret..."
kubectl create secret docker-registry klettagjadockerhub --docker-server=docker.io --docker-username=$dockerUsername --docker-password=$plainPassword
$plainPassword = $null

# SQL Server
Write-Output "Deploying SQL Server..."
kubectl kustomize --load-restrictor=LoadRestrictionsNone $baseDir/infrastructure/mssql/init-configs | kubectl apply -f $_ -
kubectl kustomize --load-restrictor=LoadRestrictionsNone $baseDir/infrastructure/mssql/sql | kubectl apply -f $_ -

Write-Output "Waiting for SQL Server to be ready..."
kubectl wait --for=condition=ready pod -l app=mssql --timeout=600s

if ($?) {
    Write-Output "SQL Server is ready."
} else {
    Write-Output "SQL Server failed to start within the given time."
    exit 1
}

if ($null -ne $deployKafka -and $deployKafka -eq $true) {
    # Kafka
    Write-Output "Deploying Kafka..."
    helm repo add bitnami https://charts.bitnami.com/bitnami
    helm repo add kafka-ui https://provectus.github.io/kafka-ui-charts
    kubectl kustomize --load-restrictor=LoadRestrictionsNone $baseDir/infrastructure/kafka/init-configs | kubectl apply -f $_ -

    helm upgrade --install kafka bitnami/kafka --version 25.3.0 `
    --values $baseDir/infrastructure/kafka/helm-values/kafka-values.yaml

    helm upgrade --install kafka-ui kafka-ui/kafka-ui --version 0.7.5 `
    --values $baseDir/infrastructure/kafka/helm-values/kafka-ui-values.yaml

    helm upgrade --install kafka bitnami/kafka --version 25.3.0 `
    --values $baseDir/infrastructure/kafka/helm-values/kafka-externalaccess.yaml `
    --reuse-values

    Write-Output "Waiting for Kafka to be ready..."
    kubectl wait --for=condition=ready pod -l app.kubernetes.io/instance=kafka --timeout=600s

}

kubectl get svc

Write-Output "Infrastructure deployment complete."

Write-Output "To check all pods are running:"
Write-Output "kubectl get pods"