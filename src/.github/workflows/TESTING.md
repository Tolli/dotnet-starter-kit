# GitHub Actions Setup - Testing Guide

## Testing the Setup Scripts

### PowerShell Script Test (Windows)

```powershell
# Navigate to the workflows directory
cd .github\workflows

# Run the setup script
.\setup-gcp.ps1
```

### Expected Behavior

#### ? First Run (Service Account Doesn't Exist)
```
=========================================
GitHub Actions GCP Setup Script
=========================================
Project ID: main-project-483817
Service Account: github-actions@main-project-483817.iam.gserviceaccount.com
=========================================

1??  Setting GCP project...
? Project set to main-project-483817

2??  Checking if service account exists...
Creating service account...
? Service account created

3??  Granting Artifact Registry Writer role...
? Artifact Registry Writer role granted

4??  Granting Kubernetes Engine Developer role...
? Kubernetes Engine Developer role granted

5??  Granting Storage Object Viewer role...
? Storage Object Viewer role granted

6??  Creating service account key...
? Service account key created: github-actions-key.json

=========================================
?? Service Account Key
=========================================

Copy the contents below and add it as a GitHub Secret named 'GCP_SA_KEY':

{
  "type": "service_account",
  ...
}

=========================================

?? Next Steps:
...
```

#### ? Second Run (Service Account Already Exists)
```
2??  Checking if service account exists...
??  Service account already exists

3??  Granting Artifact Registry Writer role...
? Artifact Registry Writer role granted
...
```

### Common Issues and Solutions

#### Issue 1: "gcloud is not installed"
```
? gcloud CLI is not installed. Please install it first:
   https://cloud.google.com/sdk/docs/install
```

**Solution:**
1. Install Google Cloud SDK from https://cloud.google.com/sdk/docs/install
2. Run `gcloud init` to configure
3. Re-run the setup script

#### Issue 2: "Permission Denied"
```
? Failed to create service account
ERROR: (gcloud.iam.service-accounts.create) PERMISSION_DENIED
```

**Solution:**
Ensure you're logged in with an account that has Owner or Service Account Admin role:
```powershell
# Check current account
gcloud auth list

# Login with correct account
gcloud auth login

# Re-run setup script
.\setup-gcp.ps1
```

#### Issue 3: "Project not found"
```
? Project 'main-project-483817' not found
```

**Solution:**
1. Verify your project ID:
```powershell
gcloud projects list
```

2. Update the `PROJECT_ID` in the script:
```powershell
# Edit setup-gcp.ps1
$PROJECT_ID = "your-actual-project-id"
```

#### Issue 4: Script exits prematurely (FIXED)
**Previous Behavior:**
```
2??  Checking if service account exists...
Script exits without error message
```

**Fixed in latest version:**
- Script now handles non-existent service accounts gracefully
- Creates the service account automatically
- Continues with remaining steps

### Manual Verification

After running the script successfully:

#### 1. Verify Service Account Exists
```powershell
gcloud iam service-accounts list --filter="email:github-actions@*"
```

Expected output:
```
DISPLAY NAME                      EMAIL
GitHub Actions Service Account    github-actions@main-project-483817.iam.gserviceaccount.com
```

#### 2. Verify Roles Assigned
```powershell
gcloud projects get-iam-policy main-project-483817 --flatten="bindings[].members" --filter="bindings.members:serviceAccount:github-actions@main-project-483817.iam.gserviceaccount.com"
```

Expected output should include:
- `roles/artifactregistry.writer`
- `roles/container.developer`
- `roles/storage.objectViewer`

#### 3. Verify Key File Created
```powershell
Test-Path github-actions-key.json
```

Should return: `True`

#### 4. Verify Key File Contents
```powershell
Get-Content github-actions-key.json | ConvertFrom-Json | Select-Object type, project_id, private_key_id
```

Expected output:
```
type            : service_account
project_id      : main-project-483817
private_key_id  : abc123...
```

## Testing GitHub Actions Workflows

### 1. Test CI Workflow

Create a test commit:
```bash
# Make a small change
echo "# Test CI" >> README.md
git add README.md
git commit -m "Test CI workflow"
git push origin main
```

**Expected:**
- Go to Actions tab
- "CI - Build and Test" workflow should trigger
- Should complete successfully

### 2. Test Server Deployment (Manual)

1. Go to your repository on GitHub
2. Click **Actions** tab
3. Select **CD - Deploy Server to Kubernetes**
4. Click **Run workflow** dropdown
5. Keep default branch (main)
6. Click **Run workflow** button

**Expected:**
- Workflow starts
- Build and push job completes
- Deploy to Kubernetes job completes
- New pods are created in your cluster

**Verify:**
```powershell
kubectl get pods -l app=fsh-webapi
kubectl get services -l app=fsh-webapi
```

### 3. Test Client Deployment (Manual)

Same as server deployment but choose **CD - Deploy Client to Kubernetes**

**Verify:**
```powershell
kubectl get pods -l app=fsh-blazor-client
kubectl get services -l app=fsh-blazor-client
```

### 4. Test Full Deployment

1. Go to **Actions** ? **Full Deploy - Server and Client**
2. Click **Run workflow**
3. Check both **Deploy Server** and **Deploy Client**
4. Click **Run workflow**

**Expected:**
- Both deployments run in parallel
- Both complete successfully

## Rollback Testing

Test that rollback works if something goes wrong:

```powershell
# Rollback server
kubectl rollout undo deployment/fsh-webapi

# Rollback client
kubectl rollout undo deployment/fsh-blazor-client

# Verify rollback
kubectl rollout status deployment/fsh-webapi
kubectl rollout status deployment/fsh-blazor-client
```

## Cleanup (Optional)

To remove test resources:

### Remove Service Account
```powershell
# List keys
gcloud iam service-accounts keys list --iam-account=github-actions@main-project-483817.iam.gserviceaccount.com

# Delete keys (replace KEY_ID)
gcloud iam service-accounts keys delete KEY_ID --iam-account=github-actions@main-project-483817.iam.gserviceaccount.com

# Delete service account
gcloud iam service-accounts delete github-actions@main-project-483817.iam.gserviceaccount.com
```

### Remove Key File
```powershell
Remove-Item github-actions-key.json
```

### Remove GitHub Secret
1. Go to repository Settings ? Secrets and variables ? Actions
2. Find `GCP_SA_KEY`
3. Click Delete

## Troubleshooting Workflows

### View Workflow Logs
1. Go to Actions tab
2. Click on the failed workflow run
3. Click on the failed job
4. Expand failed step
5. Read error message

### Common Workflow Errors

#### "Error: Invalid credentials"
- Check that `GCP_SA_KEY` secret is set correctly
- Verify JSON is not corrupted
- Ensure service account still exists

#### "Error: Cluster not found"
- Update `GKE_CLUSTER` and `GKE_ZONE` in workflow files
- Verify cluster exists: `gcloud container clusters list`

#### "Error: Permission denied"
- Service account needs more permissions
- Re-run setup script or manually grant roles

## Success Checklist

- [x] Setup script completes without errors
- [x] Service account created with correct roles
- [x] Key file generated and added to GitHub Secrets
- [x] CI workflow runs on push
- [x] Server deployment workflow succeeds
- [x] Client deployment workflow succeeds
- [x] Pods are running in Kubernetes
- [x] Application is accessible via ingress

## Need Help?

If tests fail, check:
1. Setup script output for errors
2. GitHub Actions logs
3. Kubernetes pod logs: `kubectl logs -l app=fsh-webapi`
4. This troubleshooting guide
5. Main README.md for detailed documentation
