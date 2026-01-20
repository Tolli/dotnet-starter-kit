# Description: This is a script that can be used to automatically run all necessary steps to setup sample observability stack in this repo.
#              It will deploy to the Kubernetes cluster in context.
#              Make sure you have gone through steps 1.-5. in README.md:
#              1. Get Docker Hub account
#              2. Get access for your Docker Hub account to Meniga repository in Docker Hub from your Meniga representative
#              3. Install tools
#              4. Configure hostnames
#              5. Setup platform

param(
    $namespace,
    $onlyUpdate # only updates the configs and deployment, does not delete and recreate namespace
)

# Be able to run the script from root or ps-scripts folder
$baseDir = (Get-Location).Path
if ($baseDir -like "*\ps-scripts") {
    $baseDir = $baseDir -replace "\\ps-scripts", ""
}

if ($null -eq $namespace) {
    $namespace = "meniga-getting-started-monitoring"
}

Write-Output "!!!WARNING!!!"
Write-Output "Make sure you have gone through steps of deploying the Meniga solution first before you deploy the observability stack."
Write-Output "Current context is:"
kubectl config current-context
Write-Output "Current namespace is:"
Write-Output $namespace
Write-Output "This script will deploy observability stack including Promtail, Loki, kube-prometheus-stack (Prometheus, Grafana, kube-state-metrics, Prometheus node exporter)"

$menigaHubChartVersion = "0.6.1"
$insightFactoryChartVersion = "0.6.3"

# fetching the DNS resolver IP
$dnsResolver=kubectl get svc -n kube-system kube-dns -o jsonpath='{.spec.clusterIP}'

# update Meniga Applications with observability stack values (Metrics, Logs, Tracing)
helm upgrade --install meniga-hub oci://registry-1.docker.io/menigaehf/meniga-hub --version $menigaHubChartVersion --values $baseDir/meniga/meniga-hub/meniga-hub-values-obs.yaml --set dnsResolver=$dnsResolver --force

helm upgrade --install cashflow-assistant oci://registry-1.docker.io/menigaehf/cashflow-assistant --values $baseDir/meniga/cashflow-assistant/cashflow-assistant-values-obs.yaml --force

helm upgrade --install insights-factory oci://registry-1.docker.io/menigaehf/insights-factory --version $insightFactoryChartVersion --values $baseDir/meniga/insights-factory/insights-factory-values-obs.yaml --force

$namespaceExists = kubectl get ns | Select-String $namespace
if ($namespaceExists) {
    Write-Output "namespace $namespace exists, deploying to it..."
}
else {
    # Create namespace
    Write-Output "Creating namespace $namespace..."
    kubectl create namespace $namespace
}

# add grafana helm repo

helm repo add grafana https://grafana.github.io/helm-charts
helm repo update
helm upgrade --install promtail grafana/promtail --values $baseDir/infrastructure/observability-stack/promtail/values.yaml -n $namespace
helm upgrade --install tempo grafana/tempo --values $baseDir/infrastructure/observability-stack/tempo/values.yaml -n $namespace
helm upgrade --install loki grafana/loki --values $baseDir/infrastructure/observability-stack/loki/values.yaml  -n $namespace
helm repo add prometheus-community https://prometheus-community.github.io/helm-charts
helm repo update
helm upgrade --install kube-prometheus-stack prometheus-community/kube-prometheus-stack --values $baseDir/infrastructure/observability-stack/kube-prometheus-stack/values.yaml -n $namespace

kubectl apply -f $baseDir/infrastructure/observability-stack/sql-exporter/values.yaml -n $namespace
# kubectl apply -f $baseDir/infrastructure/observability-stack/cadvisor/values.yaml -n $namespace
kubectl kustomize --load-restrictor=LoadRestrictionsNone $baseDir/infrastructure/observability-stack/dashboards | kubectl apply -f $_ - -n $namespace

Write-Output "All services are deployed. Deployment complete."

Write-Output "To check all pods are running:"
Write-Output "kubectl get pods -n $namespace"
