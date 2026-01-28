# Setup script for GitHub Actions with Workload Identity Federation (PowerShell)

# Don't stop on all errors - we'll handle them explicitly
$ErrorActionPreference = "Continue"

# Configuration
$PROJECT_ID = "main-project-483817"
$PROJECT_NUMBER = "" # Will be fetched automatically
$SERVICE_ACCOUNT_NAME = "github-actions"
$SERVICE_ACCOUNT_EMAIL = "${SERVICE_ACCOUNT_NAME}@${PROJECT_ID}.iam.gserviceaccount.com"
$WORKLOAD_IDENTITY_POOL = "github-pool"
$WORKLOAD_IDENTITY_PROVIDER = "github-provider"
$GITHUB_REPO = "Tolli/dotnet-starter-kit"  # ?? UPDATE THIS: Your GitHub username/repo

Write-Host "=========================================" -ForegroundColor Cyan
Write-Host "GitHub Actions GCP Setup (Workload Identity)" -ForegroundColor Cyan
Write-Host "=========================================" -ForegroundColor Cyan
Write-Host "Project ID: $PROJECT_ID" -ForegroundColor White
Write-Host "GitHub Repo: $GITHUB_REPO" -ForegroundColor White
Write-Host "Service Account: $SERVICE_ACCOUNT_EMAIL" -ForegroundColor White
Write-Host "=========================================" -ForegroundColor Cyan
Write-Host ""
Write-Host "??  This setup uses Workload Identity Federation" -ForegroundColor Yellow
Write-Host "   (More secure than service account keys)" -ForegroundColor Yellow
Write-Host ""

# Validate GitHub repo format
if ($GITHUB_REPO -notmatch '^[a-zA-Z0-9-]+/[a-zA-Z0-9-_.]+$') {
    Write-Host "? Invalid GitHub repository format: $GITHUB_REPO" -ForegroundColor Red
    Write-Host "   Expected format: username/repository" -ForegroundColor Yellow
    Write-Host "   Example: Tolli/dotnet-starter-kit" -ForegroundColor Yellow
    exit 1
}

$repoOwner = $GITHUB_REPO.Split('/')[0]
$repoName = $GITHUB_REPO.Split('/')[1]
Write-Host "? GitHub repository validated" -ForegroundColor Green
Write-Host "   Owner: $repoOwner" -ForegroundColor Gray
Write-Host "   Repository: $repoName" -ForegroundColor Gray
Write-Host ""

# Check if gcloud is installed
if (!(Get-Command gcloud -ErrorAction SilentlyContinue)) {
    Write-Host "? gcloud CLI is not installed. Please install it first:" -ForegroundColor Red
    Write-Host "   https://cloud.google.com/sdk/docs/install" -ForegroundColor Yellow
    exit 1
}

# Set the project
Write-Host "1??  Setting GCP project..." -ForegroundColor Yellow
gcloud config set project $PROJECT_ID | Out-Null
if ($LASTEXITCODE -ne 0) {
    Write-Host "? Failed to set project" -ForegroundColor Red
    exit 1
}
Write-Host "? Project set to $PROJECT_ID" -ForegroundColor Green
Write-Host ""

# Get project number
Write-Host "2??  Getting project number..." -ForegroundColor Yellow
$PROJECT_NUMBER = gcloud projects describe $PROJECT_ID --format="value(projectNumber)"
if ([string]::IsNullOrEmpty($PROJECT_NUMBER)) {
    Write-Host "? Failed to get project number" -ForegroundColor Red
    exit 1
}
Write-Host "? Project number: $PROJECT_NUMBER" -ForegroundColor Green
Write-Host ""

# Enable required APIs
Write-Host "3??  Enabling required APIs..." -ForegroundColor Yellow
$apis = @(
    "iam.googleapis.com",
    "cloudresourcemanager.googleapis.com",
    "iamcredentials.googleapis.com",
    "sts.googleapis.com"
)

foreach ($api in $apis) {
    Write-Host "   Enabling $api..." -ForegroundColor Gray
    gcloud services enable $api --project=$PROJECT_ID 2>&1 | Out-Null
}
Write-Host "? APIs enabled" -ForegroundColor Green
Write-Host ""

# Check if service account exists
Write-Host "4??  Checking if service account exists..." -ForegroundColor Yellow
try {
    $ErrorActionPreference = "SilentlyContinue"
    $saExists = gcloud iam service-accounts describe $SERVICE_ACCOUNT_EMAIL 2>&1
    $ErrorActionPreference = "Continue"
    
    if ($LASTEXITCODE -eq 0) {
        Write-Host "??  Service account already exists" -ForegroundColor Yellow
    } else {
        Write-Host "Creating service account..." -ForegroundColor White
        gcloud iam service-accounts create $SERVICE_ACCOUNT_NAME `
            --display-name="GitHub Actions Service Account" `
            --description="Service account for GitHub Actions CI/CD (Workload Identity)"
        
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
        --description="Service account for GitHub Actions CI/CD (Workload Identity)"
    
    if ($LASTEXITCODE -ne 0) {
        Write-Host "? Failed to create service account" -ForegroundColor Red
        exit 1
    }
    Write-Host "? Service account created" -ForegroundColor Green
}
Write-Host ""

# Grant roles to service account
Write-Host "5??  Granting roles to service account..." -ForegroundColor Yellow

$roles = @(
    "roles/artifactregistry.writer",
    "roles/container.developer",
    "roles/storage.objectViewer"
)

foreach ($role in $roles) {
    Write-Host "   Granting $role..." -ForegroundColor Gray
    gcloud projects add-iam-policy-binding $PROJECT_ID `
        --member="serviceAccount:$SERVICE_ACCOUNT_EMAIL" `
        --role="$role" `
        --condition=None `
        --quiet 2>&1 | Out-Null
}
Write-Host "? Roles granted" -ForegroundColor Green
Write-Host ""

# Create Workload Identity Pool
Write-Host "6??  Creating Workload Identity Pool..." -ForegroundColor Yellow
$poolExists = gcloud iam workload-identity-pools describe $WORKLOAD_IDENTITY_POOL `
    --location="global" 2>&1

if ($LASTEXITCODE -eq 0) {
    Write-Host "??  Workload Identity Pool already exists" -ForegroundColor Yellow
} else {
    gcloud iam workload-identity-pools create $WORKLOAD_IDENTITY_POOL `
        --location="global" `
        --display-name="GitHub Actions Pool"
    
    if ($LASTEXITCODE -ne 0) {
        Write-Host "? Failed to create Workload Identity Pool" -ForegroundColor Red
        exit 1
    }
    Write-Host "? Workload Identity Pool created" -ForegroundColor Green
}
Write-Host ""

# Create Workload Identity Provider
Write-Host "7??  Creating Workload Identity Provider..." -ForegroundColor Yellow
$providerExists = gcloud iam workload-identity-pools providers describe $WORKLOAD_IDENTITY_PROVIDER `
    --location="global" `
    --workload-identity-pool=$WORKLOAD_IDENTITY_POOL 2>&1

if ($LASTEXITCODE -eq 0) {
    Write-Host "??  Workload Identity Provider already exists" -ForegroundColor Yellow
} else {
    gcloud iam workload-identity-pools providers create-oidc $WORKLOAD_IDENTITY_PROVIDER `
        --location="global" `
        --workload-identity-pool=$WORKLOAD_IDENTITY_POOL `
        --display-name="GitHub Provider" `
        --attribute-mapping="google.subject=assertion.sub,attribute.actor=assertion.actor,attribute.repository=assertion.repository,attribute.repository_owner=assertion.repository_owner" `
        --attribute-condition="assertion.repository_owner == '$($GITHUB_REPO.Split('/')[0])'" `
        --issuer-uri="https://token.actions.githubusercontent.com"
    
    if ($LASTEXITCODE -ne 0) {
        Write-Host "? Failed to create Workload Identity Provider" -ForegroundColor Red
        Write-Host "   Trying without attribute condition..." -ForegroundColor Yellow
        
        # Try without condition if it fails
        gcloud iam workload-identity-pools providers create-oidc $WORKLOAD_IDENTITY_PROVIDER `
            --location="global" `
            --workload-identity-pool=$WORKLOAD_IDENTITY_POOL `
            --display-name="GitHub Provider" `
            --attribute-mapping="google.subject=assertion.sub,attribute.actor=assertion.actor,attribute.repository=assertion.repository,attribute.repository_owner=assertion.repository_owner" `
            --issuer-uri="https://token.actions.githubusercontent.com"
        
        if ($LASTEXITCODE -ne 0) {
            Write-Host "? Failed to create Workload Identity Provider" -ForegroundColor Red
            exit 1
        }
    }
    Write-Host "? Workload Identity Provider created" -ForegroundColor Green
}
Write-Host ""

# Allow GitHub Actions to impersonate the service account
Write-Host "8??  Configuring service account impersonation..." -ForegroundColor Yellow
gcloud iam service-accounts add-iam-policy-binding $SERVICE_ACCOUNT_EMAIL `
    --role="roles/iam.workloadIdentityUser" `
    --member="principalSet://iam.googleapis.com/projects/$PROJECT_NUMBER/locations/global/workloadIdentityPools/$WORKLOAD_IDENTITY_POOL/attribute.repository/$GITHUB_REPO"

if ($LASTEXITCODE -ne 0) {
    Write-Host "? Failed to configure impersonation" -ForegroundColor Red
    exit 1
}
Write-Host "? Service account impersonation configured" -ForegroundColor Green
Write-Host ""

# Get Workload Identity Provider name
$WORKLOAD_IDENTITY_PROVIDER_FULL = "projects/$PROJECT_NUMBER/locations/global/workloadIdentityPools/$WORKLOAD_IDENTITY_POOL/providers/$WORKLOAD_IDENTITY_PROVIDER"

# Display configuration
Write-Host "=========================================" -ForegroundColor Cyan
Write-Host "?? Configuration Complete!" -ForegroundColor Cyan
Write-Host "=========================================" -ForegroundColor Cyan
Write-Host ""
Write-Host "Add these secrets to your GitHub repository:" -ForegroundColor Yellow
Write-Host ""
Write-Host "1. Go to: https://github.com/$GITHUB_REPO/settings/secrets/actions" -ForegroundColor White
Write-Host ""
Write-Host "2. Add these secrets:" -ForegroundColor White
Write-Host ""
Write-Host "   Name: GCP_PROJECT_ID" -ForegroundColor Cyan
Write-Host "   Value: $PROJECT_ID" -ForegroundColor White
Write-Host ""
Write-Host "   Name: GCP_SERVICE_ACCOUNT" -ForegroundColor Cyan
Write-Host "   Value: $SERVICE_ACCOUNT_EMAIL" -ForegroundColor White
Write-Host ""
Write-Host "   Name: GCP_WORKLOAD_IDENTITY_PROVIDER" -ForegroundColor Cyan
Write-Host "   Value: $WORKLOAD_IDENTITY_PROVIDER_FULL" -ForegroundColor White
Write-Host ""
Write-Host "=========================================" -ForegroundColor Cyan
Write-Host ""

# Save to file for easy copying
$configFile = "github-actions-config.txt"
@"
GitHub Actions Configuration for Workload Identity Federation
==============================================================

Add these secrets to your GitHub repository:
https://github.com/$GITHUB_REPO/settings/secrets/actions

GCP_PROJECT_ID
$PROJECT_ID

GCP_SERVICE_ACCOUNT
$SERVICE_ACCOUNT_EMAIL

GCP_WORKLOAD_IDENTITY_PROVIDER
$WORKLOAD_IDENTITY_PROVIDER_FULL

==============================================================
"@ | Out-File -FilePath $configFile -Encoding UTF8

Write-Host "? Configuration saved to: $configFile" -ForegroundColor Green
Write-Host ""
Write-Host "?? Next Steps:" -ForegroundColor Cyan
Write-Host "1. Add the three secrets above to GitHub" -ForegroundColor White
Write-Host "2. Update your workflow files to use Workload Identity" -ForegroundColor White
Write-Host "3. Push changes and test deployment" -ForegroundColor White
Write-Host ""
Write-Host "?? Setup complete! (No JSON keys needed!)" -ForegroundColor Green
Write-Host ""
