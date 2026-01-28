# Setup script for GitHub Actions GCP Service Account (PowerShell)

# Don't stop on all errors - we'll handle them explicitly
$ErrorActionPreference = "Continue"

# Configuration
$PROJECT_ID = "main-project-483817"
$SERVICE_ACCOUNT_NAME = "github-actions"
$SERVICE_ACCOUNT_EMAIL = "$SERVICE_ACCOUNT_NAME@$PROJECT_ID.iam.gserviceaccount.com"
$KEY_FILE = "github-actions-key.json"

Write-Host "=========================================" -ForegroundColor Cyan
Write-Host "GitHub Actions GCP Setup Script" -ForegroundColor Cyan
Write-Host "=========================================" -ForegroundColor Cyan
Write-Host "Project ID: $PROJECT_ID"
Write-Host "Service Account: $SERVICE_ACCOUNT_EMAIL"
Write-Host "=========================================" -ForegroundColor Cyan
Write-Host ""

# Check if gcloud is installed
if (!(Get-Command gcloud -ErrorAction SilentlyContinue)) {
    Write-Host "? gcloud CLI is not installed. Please install it first:" -ForegroundColor Red
    Write-Host "   https://cloud.google.com/sdk/docs/install"
    exit 1
}

# Set the project
Write-Host "1??  Setting GCP project..." -ForegroundColor Yellow
gcloud config set project $PROJECT_ID
Write-Host "? Project set to $PROJECT_ID" -ForegroundColor Green
Write-Host ""

# Check if service account exists
Write-Host "2??  Checking if service account exists..." -ForegroundColor Yellow
try {
    $ErrorActionPreference = "SilentlyContinue"
    $saExists = gcloud iam service-accounts describe $SERVICE_ACCOUNT_EMAIL 2>&1
    $ErrorActionPreference = "Stop"
    
    if ($LASTEXITCODE -eq 0) {
        Write-Host "??  Service account already exists" -ForegroundColor Yellow
    } else {
        Write-Host "Creating service account..." -ForegroundColor White
        gcloud iam service-accounts create $SERVICE_ACCOUNT_NAME `
            --display-name="GitHub Actions Service Account" `
            --description="Service account for GitHub Actions CI/CD"
        
        if ($LASTEXITCODE -ne 0) {
            Write-Host "? Failed to create service account" -ForegroundColor Red
            exit 1
        }
        Write-Host "? Service account created" -ForegroundColor Green
    }
} catch {
    Write-Host "Creating service account..." -ForegroundColor White
    gcloud iam service-accounts create $SERVICE_ACCOUNT_NAME `
        --display-name="GitHub Actions Service Account" `
        --description="Service account for GitHub Actions CI/CD"
    
    if ($LASTEXITCODE -ne 0) {
        Write-Host "? Failed to create service account" -ForegroundColor Red
        exit 1
    }
    Write-Host "? Service account created" -ForegroundColor Green
}
Write-Host ""

# Grant Artifact Registry Writer role
Write-Host "3??  Granting Artifact Registry Writer role..." -ForegroundColor Yellow
gcloud projects add-iam-policy-binding $PROJECT_ID `
    --member="serviceAccount:$SERVICE_ACCOUNT_EMAIL" `
    --role="roles/artifactregistry.writer" `
    --condition=None `
    --quiet | Out-Null
Write-Host "? Artifact Registry Writer role granted" -ForegroundColor Green
Write-Host ""

# Grant Container Developer role (for GKE deployments)
Write-Host "4??  Granting Kubernetes Engine Developer role..." -ForegroundColor Yellow
gcloud projects add-iam-policy-binding $PROJECT_ID `
    --member="serviceAccount:$SERVICE_ACCOUNT_EMAIL" `
    --role="roles/container.developer" `
    --condition=None `
    --quiet | Out-Null
Write-Host "? Kubernetes Engine Developer role granted" -ForegroundColor Green
Write-Host ""

# Grant Storage Object Viewer role (for GKE)
Write-Host "5??  Granting Storage Object Viewer role..." -ForegroundColor Yellow
gcloud projects add-iam-policy-binding $PROJECT_ID `
    --member="serviceAccount:$SERVICE_ACCOUNT_EMAIL" `
    --role="roles/storage.objectViewer" `
    --condition=None `
    --quiet | Out-Null
Write-Host "? Storage Object Viewer role granted" -ForegroundColor Green
Write-Host ""

# Create and download key
Write-Host "6??  Creating service account key..." -ForegroundColor Yellow
if (Test-Path $KEY_FILE) {
    Write-Host "??  Key file already exists. Backing up..." -ForegroundColor Yellow
    $timestamp = Get-Date -Format "yyyyMMddHHmmss"
    Move-Item $KEY_FILE "$KEY_FILE.backup.$timestamp"
}

gcloud iam service-accounts keys create $KEY_FILE `
    --iam-account=$SERVICE_ACCOUNT_EMAIL

if ($LASTEXITCODE -ne 0) {
    Write-Host "? Failed to create service account key" -ForegroundColor Red
    Write-Host "Please check that:" -ForegroundColor Yellow
    Write-Host "  - The service account exists" -ForegroundColor Yellow
    Write-Host "  - You have permission to create keys" -ForegroundColor Yellow
    Write-Host "  - The project ID is correct" -ForegroundColor Yellow
    exit 1
}

if (!(Test-Path $KEY_FILE)) {
    Write-Host "? Key file was not created" -ForegroundColor Red
    exit 1
}

Write-Host "? Service account key created: $KEY_FILE" -ForegroundColor Green
Write-Host ""

# Display the key content
Write-Host "=========================================" -ForegroundColor Cyan
Write-Host "?? Service Account Key" -ForegroundColor Cyan
Write-Host "=========================================" -ForegroundColor Cyan
Write-Host ""
Write-Host "Copy the contents below and add it as a GitHub Secret named 'GCP_SA_KEY':" -ForegroundColor Yellow
Write-Host ""
Get-Content $KEY_FILE | Write-Host
Write-Host ""
Write-Host "=========================================" -ForegroundColor Cyan
Write-Host ""

# Instructions
Write-Host "?? Next Steps:" -ForegroundColor Cyan
Write-Host ""
Write-Host "1. Go to your GitHub repository" -ForegroundColor White
Write-Host "2. Navigate to Settings ? Secrets and variables ? Actions" -ForegroundColor White
Write-Host "3. Click 'New repository secret'" -ForegroundColor White
Write-Host "4. Name: GCP_SA_KEY" -ForegroundColor White
Write-Host "5. Value: Paste the JSON content from above" -ForegroundColor White
Write-Host "6. Click 'Add secret'" -ForegroundColor White
Write-Host ""
Write-Host "7. Update the workflow files (.github/workflows/deploy-*.yml):" -ForegroundColor White
Write-Host "   - Set GKE_CLUSTER to your cluster name" -ForegroundColor Gray
Write-Host "   - Set GKE_ZONE to your cluster zone" -ForegroundColor Gray
Write-Host "   - Set NAMESPACE if not using 'default'" -ForegroundColor Gray
Write-Host ""
Write-Host "?? Setup complete! You can now use GitHub Actions to deploy." -ForegroundColor Green
Write-Host ""

# Cleanup option
$response = Read-Host "Delete the key file from local machine? (y/N)"
if ($response -eq 'y' -or $response -eq 'Y') {
    Remove-Item $KEY_FILE
    Write-Host "? Key file deleted. Make sure you saved it to GitHub Secrets!" -ForegroundColor Green
} else {
    Write-Host "??  Remember to delete $KEY_FILE after adding it to GitHub Secrets!" -ForegroundColor Yellow
}
