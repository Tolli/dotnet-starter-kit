# GitHub Actions Quick Start Guide

This guide will help you set up CI/CD for your FSH Starter Kit application in 15 minutes.

## ? Prerequisites

Before you start, ensure you have:
- [ ] GitHub repository with your code
- [ ] Google Cloud Platform account
- [ ] GKE cluster created and running
- [ ] Artifact Registry repository created
- [ ] `kubectl` configured to access your cluster

## ?? Quick Setup (5 Steps)

### Step 1: Create GCP Service Account

**Using PowerShell:**
```powershell
cd .github/workflows
.\setup-gcp.ps1
```

**Using Bash:**
```bash
cd .github/workflows
chmod +x setup-gcp.sh
./setup-gcp.sh
```

This script will:
- Create a service account named `github-actions`
- Grant required permissions
- Generate a JSON key file
- Display the key content

### Step 2: Add GitHub Secret

1. Go to your GitHub repository
2. Navigate to **Settings** ? **Secrets and variables** ? **Actions**
3. Click **New repository secret**
4. Name: `GCP_SA_KEY`
5. Value: Paste the entire JSON content from Step 1
6. Click **Add secret**

### Step 3: Update Workflow Configuration

Edit these files and update the values:

**`.github/workflows/deploy-server.yml`**
```yaml
env:
  GKE_CLUSTER: your-actual-cluster-name    # ?? CHANGE THIS
  GKE_ZONE: europe-west9-a                 # ?? CHANGE THIS
  NAMESPACE: default                       # ?? CHANGE if needed
```

**`.github/workflows/deploy-client.yml`**
```yaml
env:
  GKE_CLUSTER: your-actual-cluster-name    # ?? CHANGE THIS
  GKE_ZONE: europe-west9-a                 # ?? CHANGE THIS
  NAMESPACE: default                       # ?? CHANGE if needed
```

**To find your cluster name and zone:**
```bash
gcloud container clusters list
```

### Step 4: Commit and Push Workflows

```bash
git add .github/
git commit -m "Add GitHub Actions CI/CD workflows"
git push origin main
```

### Step 5: Verify Setup

1. Go to your repository on GitHub
2. Click the **Actions** tab
3. You should see the workflows listed:
   - ? CI - Build and Test
   - ? CD - Deploy Server to Kubernetes
   - ? CD - Deploy Client to Kubernetes
   - ? Full Deploy - Server and Client

## ?? Test Your Setup

### Test Manual Deployment

1. Go to **Actions** ? **Full Deploy - Server and Client**
2. Click **Run workflow**
3. Keep both checkboxes selected
4. Click **Run workflow** button
5. Watch the deployment progress

### Test Automatic Deployment

Make a small change to your code and push:

```bash
# For testing server deployment
echo "# Test" >> api/server/Program.cs
git add api/server/Program.cs
git commit -m "Test server deployment"
git push origin main
```

The **CD - Deploy Server to Kubernetes** workflow should automatically trigger.

## ?? Monitor Deployments

### View Workflow Status
- Go to **Actions** tab
- Click on the running workflow
- Expand each step to see detailed logs

### View Deployed Application
```bash
# Check server pods
kubectl get pods -l app=fsh-webapi

# Check client pods
kubectl get pods -l app=fsh-blazor-client

# View server logs
kubectl logs -l app=fsh-webapi --tail=50

# View client logs
kubectl logs -l app=fsh-blazor-client --tail=50

# Check services
kubectl get services

# Check ingress
kubectl get ingress
```

## ?? Troubleshooting

### Workflow fails: "Error: Not Found"
**Problem:** GKE cluster name or zone is incorrect
**Solution:** 
```bash
# List your clusters
gcloud container clusters list

# Update workflow files with correct names
```

### Workflow fails: "Permission Denied"
**Problem:** Service account doesn't have required permissions
**Solution:** Re-run the setup script or manually grant permissions:
```bash
gcloud projects add-iam-policy-binding main-project-483817 \
  --member="serviceAccount:github-actions@main-project-483817.iam.gserviceaccount.com" \
  --role="roles/container.developer"
```

### Deployment succeeds but application doesn't work
**Problem:** ConfigMap or Secrets not configured correctly
**Solution:** 
- Check ConfigMap values in `k8s/server-configmap.yaml` and `k8s/blazor-configmap.yaml`
- Verify Secrets in `k8s/server-secret.yaml` and `k8s/blazor-secret.yaml`
- Check pod logs for errors

## ?? What Happens After Setup?

### Automatic Deployments

| Trigger | Action |
|---------|--------|
| Push to `main` (changes in `api/**`) | Server deployment |
| Push to `main` (changes in `apps/blazor/**`) | Client deployment |
| Pull Request to `main` or `develop` | CI build and test |

### Manual Deployments

You can manually trigger deployments:
1. Go to **Actions**
2. Select the workflow
3. Click **Run workflow**
4. Choose options
5. Click **Run workflow**

## ?? Success!

If you've completed all steps:
- ? Your code builds automatically on every push
- ? Tests run automatically
- ? Server deploys automatically when API code changes
- ? Client deploys automatically when Blazor code changes
- ? You can trigger manual deployments anytime

## ?? Security Best Practices

- ? Use GitHub Secrets for sensitive data
- ? Service account has minimum required permissions
- ? Rotate service account keys regularly (every 90 days)
- ? Never commit secrets to the repository
- ? Review and audit workflow runs regularly

## ?? Next Steps

- [ ] Set up branch protection rules
- [ ] Add staging environment
- [ ] Configure deployment approvals for production
- [ ] Add Slack/Teams notifications
- [ ] Set up monitoring and alerts
- [ ] Add database migration step
- [ ] Configure blue-green or canary deployments

## ?? Need Help?

- Check the detailed [README.md](.github/workflows/README.md)
- Review GitHub Actions logs
- Check Kubernetes pod logs
- Verify ConfigMaps and Secrets
- Ensure GKE cluster is accessible

---

?? **Congratulations!** Your CI/CD pipeline is now set up and ready to use!
