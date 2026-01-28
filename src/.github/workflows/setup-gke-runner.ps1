# Setup Self-Hosted GitHub Actions Runner in GKE
# This script sets up the infrastructure needed for GitHub Actions Runner Controller (ARC)

param(
    [string]$ProjectId = "main-project-483817",
    [string]$ClusterName = "your-cluster-name",  # ?? UPDATE THIS
    [string]$ClusterZone = "europe-west9-a",     # ?? UPDATE THIS
    [string]$Namespace = "actions-runner-system",
    [string]$GitHubRepo = "Tolli/dotnet-starter-kit"  # ?? UPDATE THIS
)

$ErrorActionPreference = "Continue"

Write-Host "=========================================" -ForegroundColor Cyan
Write-Host "GitHub Actions Self-Hosted Runner Setup" -ForegroundColor Cyan
Write-Host "=========================================" -ForegroundColor Cyan
Write-Host "Project: $ProjectId" -ForegroundColor White
Write-Host "Cluster: $ClusterName" -ForegroundColor White
Write-Host "Zone: $ClusterZone" -ForegroundColor White
Write-Host "Repository: $GitHubRepo" -ForegroundColor White
Write-Host "=========================================" -ForegroundColor Cyan
Write-Host ""

# Check prerequisites
Write-Host "1??  Checking prerequisites..." -ForegroundColor Yellow

# Check gcloud
if (!(Get-Command gcloud -ErrorAction SilentlyContinue)) {
    Write-Host "? gcloud CLI is not installed" -ForegroundColor Red
    exit 1
}

# Check kubectl
if (!(Get-Command kubectl -ErrorAction SilentlyContinue)) {
    Write-Host "? kubectl is not installed" -ForegroundColor Red
    Write-Host "   Install: https://kubernetes.io/docs/tasks/tools/" -ForegroundColor Yellow
    exit 1
}

# Check helm
if (!(Get-Command helm -ErrorAction SilentlyContinue)) {
    Write-Host "? Helm is not installed" -ForegroundColor Red
    Write-Host "   Install: https://helm.sh/docs/intro/install/" -ForegroundColor Yellow
    exit 1
}

Write-Host "? Prerequisites verified" -ForegroundColor Green
Write-Host ""

# Set project
Write-Host "2??  Setting GCP project..." -ForegroundColor Yellow
gcloud config set project $ProjectId | Out-Null
Write-Host "? Project set" -ForegroundColor Green
Write-Host ""

# Get cluster credentials
Write-Host "3??  Getting GKE cluster credentials..." -ForegroundColor Yellow
gcloud container clusters get-credentials $ClusterName --zone=$ClusterZone --project=$ProjectId 2>&1 | Out-Null
if ($LASTEXITCODE -ne 0) {
    Write-Host "? Failed to get cluster credentials" -ForegroundColor Red
    Write-Host "   Make sure cluster name and zone are correct" -ForegroundColor Yellow
    exit 1
}
Write-Host "? Cluster credentials obtained" -ForegroundColor Green
Write-Host ""

# Create namespace
Write-Host "4??  Creating namespace..." -ForegroundColor Yellow
kubectl create namespace $Namespace --dry-run=client -o yaml | kubectl apply -f - 2>&1 | Out-Null
Write-Host "? Namespace created: $Namespace" -ForegroundColor Green
Write-Host ""

# Add Helm repo for Actions Runner Controller
Write-Host "5??  Adding Actions Runner Controller Helm repository..." -ForegroundColor Yellow
helm repo add actions-runner-controller https://actions-runner-controller.github.io/actions-runner-controller 2>&1 | Out-Null
helm repo update 2>&1 | Out-Null
Write-Host "? Helm repository added" -ForegroundColor Green
Write-Host ""

# Check if cert-manager is installed
Write-Host "6??  Checking for cert-manager..." -ForegroundColor Yellow
$certManagerInstalled = kubectl get namespace cert-manager 2>&1
if ($LASTEXITCODE -ne 0) {
    Write-Host "   Installing cert-manager (required by ARC)..." -ForegroundColor Gray
    kubectl apply -f https://github.com/cert-manager/cert-manager/releases/download/v1.13.0/cert-manager.yaml 2>&1 | Out-Null
    
    Write-Host "   Waiting for cert-manager to be ready..." -ForegroundColor Gray
    kubectl wait --for=condition=Available --timeout=300s deployment/cert-manager -n cert-manager 2>&1 | Out-Null
    kubectl wait --for=condition=Available --timeout=300s deployment/cert-manager-webhook -n cert-manager 2>&1 | Out-Null
    
    Write-Host "? cert-manager installed" -ForegroundColor Green
} else {
    Write-Host "? cert-manager already installed" -ForegroundColor Green
}
Write-Host ""

Write-Host "=========================================" -ForegroundColor Cyan
Write-Host "?? Next Steps - GitHub Configuration" -ForegroundColor Cyan
Write-Host "=========================================" -ForegroundColor Cyan
Write-Host ""
Write-Host "You need to create a GitHub Personal Access Token (PAT) or GitHub App" -ForegroundColor Yellow
Write-Host ""
Write-Host "Option 1: Personal Access Token (Easier)" -ForegroundColor Cyan
Write-Host "1. Go to: https://github.com/settings/tokens" -ForegroundColor White
Write-Host "2. Click 'Generate new token' ? 'Generate new token (classic)'" -ForegroundColor White
Write-Host "3. Give it a name: 'GKE Self-Hosted Runner'" -ForegroundColor White
Write-Host "4. Select scopes:" -ForegroundColor White
Write-Host "   - repo (Full control of private repositories)" -ForegroundColor Gray
Write-Host "   - workflow (Update GitHub Action workflows)" -ForegroundColor Gray
Write-Host "   - admin:org (if using organization runners)" -ForegroundColor Gray
Write-Host "5. Click 'Generate token' and copy it" -ForegroundColor White
Write-Host ""
Write-Host "Option 2: GitHub App (More Secure)" -ForegroundColor Cyan
Write-Host "1. Go to: https://github.com/settings/apps/new" -ForegroundColor White
Write-Host "2. Follow the guide in RUNNER-GITHUB-APP.md" -ForegroundColor White
Write-Host ""
Write-Host "=========================================" -ForegroundColor Cyan
Write-Host ""

# Prompt for token
Write-Host "Enter your GitHub Personal Access Token (PAT):" -ForegroundColor Yellow
$githubToken = Read-Host -AsSecureString
$githubTokenPlain = [Runtime.InteropServices.Marshal]::PtrToStringAuto(
    [Runtime.InteropServices.Marshal]::SecureStringToBSTR($githubToken)
)

if ([string]::IsNullOrWhiteSpace($githubTokenPlain)) {
    Write-Host "? GitHub token is required" -ForegroundColor Red
    exit 1
}

Write-Host ""
Write-Host "7??  Creating Kubernetes secret for GitHub authentication..." -ForegroundColor Yellow
kubectl create secret generic controller-manager `
    -n $Namespace `
    --from-literal=github_token=$githubTokenPlain `
    --dry-run=client -o yaml | kubectl apply -f - 2>&1 | Out-Null

if ($LASTEXITCODE -ne 0) {
    Write-Host "? Failed to create secret" -ForegroundColor Red
    exit 1
}
Write-Host "? Secret created" -ForegroundColor Green
Write-Host ""

# Install Actions Runner Controller
Write-Host "8??  Installing Actions Runner Controller..." -ForegroundColor Yellow
helm upgrade --install actions-runner-controller `
    actions-runner-controller/actions-runner-controller `
    --namespace $Namespace `
    --set authSecret.create=false `
    --set authSecret.name=controller-manager `
    --set authSecret.github_token=github_token `
    --wait `
    --timeout 5m 2>&1 | Out-Null

if ($LASTEXITCODE -ne 0) {
    Write-Host "? Failed to install Actions Runner Controller" -ForegroundColor Red
    exit 1
}
Write-Host "? Actions Runner Controller installed" -ForegroundColor Green
Write-Host ""

# Wait for controller to be ready
Write-Host "9??  Waiting for controller to be ready..." -ForegroundColor Yellow
kubectl wait --for=condition=Available --timeout=300s deployment/actions-runner-controller -n $Namespace 2>&1 | Out-Null
Write-Host "? Controller is ready" -ForegroundColor Green
Write-Host ""

# Create runner deployment manifest
$runnerManifest = @"
apiVersion: actions.summerwind.dev/v1alpha1
kind: RunnerDeployment
metadata:
  name: dotnet-starter-kit-runner
  namespace: $Namespace
spec:
  replicas: 2
  template:
    spec:
      repository: $GitHubRepo
      labels:
        - self-hosted
        - linux
        - x64
        - gke
      resources:
        limits:
          cpu: "2"
          memory: "4Gi"
        requests:
          cpu: "1"
          memory: "2Gi"
      # Use the official runner image with .NET support
      image: summerwind/actions-runner-dind:latest
      dockerdWithinRunnerContainer: true
      # Volume for Docker
      volumeMounts:
        - name: work
          mountPath: /runner/_work
      volumes:
        - name: work
          emptyDir: {}
"@

$runnerManifest | Out-File -FilePath "github-runner-deployment.yaml" -Encoding UTF8

Write-Host "?? Deploying GitHub Actions runners..." -ForegroundColor Yellow
kubectl apply -f github-runner-deployment.yaml 2>&1 | Out-Null
if ($LASTEXITCODE -ne 0) {
    Write-Host "? Failed to deploy runners" -ForegroundColor Red
    exit 1
}
Write-Host "? Runner deployment created" -ForegroundColor Green
Write-Host ""

# Create autoscaling configuration
$hpaManifest = @"
apiVersion: actions.summerwind.dev/v1alpha1
kind: HorizontalRunnerAutoscaler
metadata:
  name: dotnet-starter-kit-runner-autoscaler
  namespace: $Namespace
spec:
  scaleTargetRef:
    name: dotnet-starter-kit-runner
  minReplicas: 1
  maxReplicas: 10
  metrics:
  - type: TotalNumberOfQueuedAndInProgressWorkflowRuns
    repositoryNames:
    - $GitHubRepo
"@

$hpaManifest | Out-File -FilePath "github-runner-autoscaler.yaml" -Encoding UTF8

Write-Host "1??1??  Deploying autoscaler..." -ForegroundColor Yellow
kubectl apply -f github-runner-autoscaler.yaml 2>&1 | Out-Null
Write-Host "? Autoscaler deployed" -ForegroundColor Green
Write-Host ""

# Wait for runners to be ready
Write-Host "1??2??  Waiting for runners to register..." -ForegroundColor Yellow
Start-Sleep -Seconds 30
Write-Host ""

# Show status
Write-Host "=========================================" -ForegroundColor Cyan
Write-Host "? Setup Complete!" -ForegroundColor Green
Write-Host "=========================================" -ForegroundColor Cyan
Write-Host ""
Write-Host "?? Runner Status:" -ForegroundColor Cyan
kubectl get runnerdeployment -n $Namespace
Write-Host ""
kubectl get runners -n $Namespace
Write-Host ""

Write-Host "=========================================" -ForegroundColor Cyan
Write-Host "?? Verification Steps" -ForegroundColor Cyan
Write-Host "=========================================" -ForegroundColor Cyan
Write-Host ""
Write-Host "1. Check runners in GitHub:" -ForegroundColor Yellow
Write-Host "   https://github.com/$GitHubRepo/settings/actions/runners" -ForegroundColor White
Write-Host ""
Write-Host "2. View runner logs:" -ForegroundColor Yellow
Write-Host "   kubectl logs -l app.kubernetes.io/name=actions-runner -n $Namespace" -ForegroundColor White
Write-Host ""
Write-Host "3. Check autoscaler:" -ForegroundColor Yellow
Write-Host "   kubectl get hra -n $Namespace" -ForegroundColor White
Write-Host ""
Write-Host "4. Test with a workflow:" -ForegroundColor Yellow
Write-Host "   - Add 'runs-on: self-hosted' to a workflow" -ForegroundColor White
Write-Host "   - Or 'runs-on: [self-hosted, linux, x64, gke]'" -ForegroundColor White
Write-Host ""

# Save configuration
$configFile = "runner-config.txt"
@"
GitHub Actions Self-Hosted Runner Configuration
================================================

Cluster: $ClusterName
Zone: $ClusterZone
Namespace: $Namespace
Repository: $GitHubRepo

Kubernetes Resources Created:
- Namespace: $Namespace
- Secret: controller-manager
- Deployment: dotnet-starter-kit-runner
- HorizontalRunnerAutoscaler: dotnet-starter-kit-runner-autoscaler

View runners in GitHub:
https://github.com/$GitHubRepo/settings/actions/runners

Useful Commands:
----------------
# View runner status
kubectl get runners -n $Namespace

# View runner logs
kubectl logs -l app.kubernetes.io/name=actions-runner -n $Namespace --tail=50

# Scale runners manually
kubectl scale runnerdeployment dotnet-starter-kit-runner --replicas=5 -n $Namespace

# View autoscaler status
kubectl get hra -n $Namespace -o yaml

# Delete runners
kubectl delete runnerdeployment dotnet-starter-kit-runner -n $Namespace

================================================
"@ | Out-File -FilePath $configFile -Encoding UTF8

Write-Host "? Configuration saved to: $configFile" -ForegroundColor Green
Write-Host ""
Write-Host "?? Self-hosted runners are now available!" -ForegroundColor Green
Write-Host ""
