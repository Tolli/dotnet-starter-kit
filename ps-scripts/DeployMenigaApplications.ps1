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

$menigaHubChartVersion = "0.6.1"
$insightFactoryChartVersion = "0.7.0"
$cashflowAssistantChartVersion = "0.4.0"

# fetching the DNS resolver IP
$dnsResolver=kubectl get svc -n kube-system kube-dns -o jsonpath='{.spec.clusterIP}'



# Meniga Hub
Write-Output "Deploying Meniga Hub..."
kubectl kustomize --load-restrictor=LoadRestrictionsNone $baseDir/meniga/meniga-hub/init-configs | kubectl apply -f $_ -

if ($deployCA -eq $true -or $deployIF -eq $true -or $deploySMR -eq $true) {
    helm upgrade --install meniga-hub oci://registry-1.docker.io/menigaehf/meniga-hub --version $menigaHubChartVersion -f $baseDir/meniga/meniga-hub/meniga-hub-values-all.yaml --set dnsResolver=$dnsResolver
}
else {
    helm upgrade --install meniga-hub oci://registry-1.docker.io/menigaehf/meniga-hub --version $menigaHubChartVersion -f $baseDir/meniga/meniga-hub/meniga-hub-values.yaml --set dnsResolver=$dnsResolver
}

if ($deployCA -eq $true) {
    # Cashflow Assistant
    Write-Output "Deploying Cashflow Assistant..."
    kubectl kustomize --load-restrictor=LoadRestrictionsNone .\meniga\cashflow-assistant | kubectl apply -f $_ -

    helm upgrade --install cashflow-assistant oci://registry-1.docker.io/menigaehf/cashflow-assistant --version $cashflowAssistantChartVersion --values $baseDir/meniga/cashflow-assistant/cashflow-assistant-values.yaml
}

if ($deployIF -eq $true) {
    # Insights Factory
    Write-Output "Deploying Insights Factory..."
    
    helm upgrade --install insights-factory oci://registry-1.docker.io/menigaehf/insights-factory --version $insightFactoryChartVersion --values $baseDir/meniga/insights-factory/insights-factory-values.yaml
}

if ($deploySMR -eq $true) {
    # Smart Money Rules
    Write-Output "Deploying Smart Money Rules..."
    helm upgrade --install smart-money-rules oci://registry-1.docker.io/menigaehf/smart-money-rules --values $baseDir/meniga/smart-money-rules/smart-money-rules-values.yaml
}

# Show ingress details for accessing the services
Write-Output "Retrieving ingress information and access info to the services:"
kubectl get ing --namespace $namespace

Write-Output "All services are deployed. Deployment complete."

Write-Output "To check all pods are running:"
Write-Output "kubectl get pods"