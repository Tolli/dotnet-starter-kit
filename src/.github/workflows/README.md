# GitHub Actions CI/CD Setup

This directory contains GitHub Actions workflows for Continuous Integration and Continuous Deployment of the FSH Starter Kit application.

## ?? Workflows Overview

### 1. **ci.yml** - Continuous Integration
- **Triggers**: Push and Pull Requests to `main` and `develop` branches
- **Purpose**: Build, test, and analyze code quality
- **Steps**:
  - Restore dependencies
  - Build solution
  - Run tests
  - Upload code coverage

### 2. **deploy-server.yml** - Deploy API Server
- **Triggers**: 
  - Push to `main` branch (when `api/**`, `Dockerfile`, or K8s configs change)
  - Manual trigger via `workflow_dispatch`
- **Purpose**: Build and deploy the .NET API to Kubernetes
- **Steps**:
  - Build Docker image
  - Push to Google Artifact Registry
  - Deploy to GKE cluster
  - Update ConfigMaps, Secrets, and Ingress

### 3. **deploy-client.yml** - Deploy Blazor Client
- **Triggers**: 
  - Push to `main` branch (when `apps/blazor/**`, `Dockerfile.Blazor`, or K8s configs change)
  - Manual trigger via `workflow_dispatch`
- **Purpose**: Build and deploy the Blazor WebAssembly client to Kubernetes
- **Steps**:
  - Build Docker image
  - Push to Google Artifact Registry
  - Deploy to GKE cluster
  - Update ConfigMaps, Secrets, and Ingress

### 4. **deploy-all.yml** - Deploy Both Server and Client
- **Triggers**: Manual trigger only (`workflow_dispatch`)
- **Purpose**: Deploy both server and client in one go
- **Options**: Choose to deploy server, client, or both

## ?? Required GitHub Secrets

Configure these secrets in your GitHub repository settings:

### Google Cloud Platform
```
GCP_SA_KEY
```
**Description**: Service Account JSON key with permissions to:
- Push to Artifact Registry
- Deploy to GKE cluster
- Manage Kubernetes resources

**How to create**:
```bash
# 1. Create a service account
gcloud iam service-accounts create github-actions \
  --display-name="GitHub Actions"

# 2. Grant necessary roles
gcloud projects add-iam-policy-binding main-project-483817 \
  --member="serviceAccount:github-actions@main-project-483817.iam.gserviceaccount.com" \
  --role="roles/artifactregistry.writer"

gcloud projects add-iam-policy-binding main-project-483817 \
  --member="serviceAccount:github-actions@main-project-483817.iam.gserviceaccount.com" \
  --role="roles/container.developer"

# 3. Create and download key
gcloud iam service-accounts keys create github-actions-key.json \
  --iam-account=github-actions@main-project-483817.iam.gserviceaccount.com

# 4. Copy the contents of github-actions-key.json to GitHub Secret GCP_SA_KEY
```

### Kubernetes Secrets (Optional)
If you want to manage K8s secrets via GitHub instead of committing them:

```
K8S_DB_CONNECTION_STRING
K8S_JWT_KEY
K8S_MAIL_PASSWORD
K8S_HANGFIRE_PASSWORD
```

## ?? Configuration

### Update Workflow Variables

Edit the workflow files and update these variables:

#### In `deploy-server.yml` and `deploy-client.yml`:

```yaml
env:
  GCP_PROJECT_ID: main-project-483817        # Your GCP project
  GCP_REGION: europe-west9                   # Your region
  GKE_CLUSTER: your-cluster-name             # ?? UPDATE THIS
  GKE_ZONE: europe-west9-a                   # ?? UPDATE THIS
  GAR_LOCATION: europe-west9                 # Artifact Registry location
  GAR_REPOSITORY: klettagja                  # Your repository name
  NAMESPACE: default                         # ?? UPDATE if using different namespace
```

### Update Kubernetes Manifests

Ensure these files exist in your `k8s/` directory:

**For Server:**
- `k8s/server-configmap.yaml`
- `k8s/server-secret.yaml`
- `k8s/server-deployment.yaml`
- `k8s/server-service.yaml`
- `k8s/server-ingress.yaml`

**For Client:**
- `k8s/blazor-configmap.yaml`
- `k8s/blazor-secret.yaml`
- `k8s/blazor-deployment.yaml`
- `k8s/blazor-service.yaml`
- `k8s/blazor-ingress.yaml`

## ?? Usage

### Automatic Deployments

When you push code to the `main` branch:
- Changes in `api/**` ? Server deployment triggers
- Changes in `apps/blazor/**` ? Client deployment triggers
- Both are deployed independently and in parallel

### Manual Deployments

#### Deploy Everything
1. Go to **Actions** ? **Full Deploy - Server and Client**
2. Click **Run workflow**
3. Choose which components to deploy
4. Click **Run workflow**

#### Deploy Server Only
1. Go to **Actions** ? **CD - Deploy Server to Kubernetes**
2. Click **Run workflow**
3. Select environment (production/staging)
4. Click **Run workflow**

#### Deploy Client Only
1. Go to **Actions** ? **CD - Deploy Client to Kubernetes**
2. Click **Run workflow**
3. Select environment (production/staging)
4. Click **Run workflow**

## ?? Monitoring Deployments

### View Workflow Runs
1. Go to **Actions** tab in your repository
2. Select the workflow you want to monitor
3. Click on a specific run to see details

### Deployment Logs
Each deployment job provides:
- Build logs
- Docker image details
- Kubernetes rollout status
- Pod and service information

### Verify Deployment
After successful deployment, verify:

```bash
# Check pods
kubectl get pods -l app=fsh-webapi
kubectl get pods -l app=fsh-blazor-client

# Check services
kubectl get services

# Check ingress
kubectl get ingress

# View logs
kubectl logs -l app=fsh-webapi --tail=50
kubectl logs -l app=fsh-blazor-client --tail=50
```

## ?? Rollback

If a deployment fails or introduces issues:

### Via kubectl
```bash
# Rollback server deployment
kubectl rollout undo deployment/fsh-webapi

# Rollback client deployment
kubectl rollout undo deployment/fsh-blazor-client

# Rollback to specific revision
kubectl rollout undo deployment/fsh-webapi --to-revision=2
```

### Via Re-deployment
1. Revert the commit in git
2. Push to `main` branch
3. Workflows will automatically deploy the previous version

## ??? Troubleshooting

### Workflow Fails at "Authenticate to Google Cloud"
**Problem**: GCP_SA_KEY secret is missing or invalid
**Solution**: 
- Verify the secret exists in repository settings
- Ensure the service account key is valid
- Check service account has required permissions

### Workflow Fails at "Get GKE credentials"
**Problem**: Cannot access GKE cluster
**Solution**:
- Update `GKE_CLUSTER` and `GKE_ZONE` in workflow files
- Ensure service account has `container.developer` role
- Verify cluster exists and is accessible

### Workflow Fails at "Build and push Docker image"
**Problem**: Cannot push to Artifact Registry
**Solution**:
- Verify Artifact Registry exists
- Ensure service account has `artifactregistry.writer` role
- Check `GAR_LOCATION` and `GAR_REPOSITORY` are correct

### Deployment Succeeds but Pods Crash
**Problem**: Application configuration issues
**Solution**:
- Check pod logs: `kubectl logs -l app=fsh-webapi`
- Verify ConfigMaps and Secrets are correct
- Ensure database connection string is valid
- Check CORS configuration

### Image Pull Errors
**Problem**: Kubernetes can't pull the Docker image
**Solution**:
- Verify image tag in deployment matches what was pushed
- Ensure GKE has access to Artifact Registry
- Check `imagePullPolicy` in deployment yaml

## ?? Best Practices

### Branch Strategy
- **main**: Production deployments
- **develop**: Development/staging deployments
- **feature/***: Feature branches (CI only, no deployment)

### Secrets Management
- ? Store sensitive data in GitHub Secrets
- ? Use Kubernetes Secrets for application secrets
- ? Never commit secrets to the repository

### Image Tagging
- `latest`: Latest build from main branch
- `main-{sha}`: Specific commit from main
- `develop-{sha}`: Specific commit from develop

### Deployment Verification
Always verify deployments with:
```bash
# Check rollout status
kubectl rollout status deployment/fsh-webapi

# Check pods are running
kubectl get pods -l app=fsh-webapi

# Check application logs
kubectl logs -l app=fsh-webapi --tail=50
```

## ?? Security Considerations

1. **Secrets**: Use GitHub Secrets and Kubernetes Secrets
2. **Service Account**: Use least privilege principle
3. **Image Scanning**: Consider adding Trivy or similar for vulnerability scanning
4. **Network Policies**: Implement Kubernetes network policies
5. **RBAC**: Configure proper RBAC for deployments

## ?? Additional Resources

- [GitHub Actions Documentation](https://docs.github.com/en/actions)
- [Google Cloud Artifact Registry](https://cloud.google.com/artifact-registry/docs)
- [Google Kubernetes Engine](https://cloud.google.com/kubernetes-engine/docs)
- [Kubernetes Deployments](https://kubernetes.io/docs/concepts/workloads/controllers/deployment/)

## ?? Support

If you encounter issues:
1. Check workflow logs in GitHub Actions
2. Review pod logs in Kubernetes
3. Verify all secrets and configurations are correct
4. Consult the troubleshooting section above
