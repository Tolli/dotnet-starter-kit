# GitHub Actions Self-Hosted Runner in GKE

## Overview

This guide helps you set up self-hosted GitHub Actions runners in your Google Kubernetes Engine (GKE) cluster using the **Actions Runner Controller (ARC)**.

## Why Self-Hosted Runners?

### Benefits
- ? **Faster builds** - Runners in same network as your resources
- ? **Cost savings** - No GitHub Actions minutes charges
- ? **More control** - Custom tools, dependencies, and configurations
- ? **Better caching** - Persistent storage for build caches
- ? **Private resources** - Access to internal services
- ? **Scalability** - Auto-scale based on workflow queue

### Use Cases
- Large monorepo builds
- Docker image builds (avoid rate limits)
- Access to private GCP resources
- Custom tooling requirements
- High-frequency CI/CD

## Prerequisites

Before you begin, ensure you have:
- [x] GKE cluster running
- [x] `kubectl` installed and configured
- [x] `helm` installed (v3+)
- [x] `gcloud` CLI installed
- [x] GitHub repository admin access

## Quick Setup

### Step 1: Run the Setup Script

```powershell
cd .github\workflows
.\setup-gke-runner.ps1
```

**What it does:**
1. Installs cert-manager (if needed)
2. Adds Actions Runner Controller Helm repo
3. Creates namespace for runners
4. Prompts for GitHub Personal Access Token
5. Installs Actions Runner Controller
6. Deploys runner pods
7. Configures autoscaling

### Step 2: Create GitHub Personal Access Token

1. Go to https://github.com/settings/tokens
2. Click **Generate new token** ? **Generate new token (classic)**
3. Name: `GKE Self-Hosted Runner`
4. Select scopes:
   - ? `repo` (Full control of private repositories)
   - ? `workflow` (Update GitHub Action workflows)
   - ? `admin:org` (if using organization runners)
5. Click **Generate token**
6. Copy the token (you'll need it for the script)

### Step 3: Verify Installation

Check runners are registered in GitHub:
```
https://github.com/Tolli/dotnet-starter-kit/settings/actions/runners
```

You should see runners with names like:
- `dotnet-starter-kit-runner-abcd1-xxxxx`
- Status: **Idle** (green)

## Using Self-Hosted Runners

### In Your Workflows

Add `runs-on: self-hosted` to your job:

```yaml
jobs:
  build:
    runs-on: self-hosted  # Simple
    # OR
    runs-on: [self-hosted, linux, x64, gke]  # With labels
    
    steps:
    - uses: actions/checkout@v4
    - name: Build
      run: dotnet build
```

### Example Workflows Created

- ? `.github/workflows/ci-self-hosted.yml` - CI build using self-hosted runners
- ? `.github/workflows/deploy-server.yml` - Can be updated to use self-hosted
- ? `.github/workflows/deploy-client.yml` - Can be updated to use self-hosted

## Configuration

### Scaling

The setup includes automatic scaling based on workflow queue:

```yaml
# HorizontalRunnerAutoscaler configuration
minReplicas: 1   # Always have 1 runner ready
maxReplicas: 10  # Scale up to 10 runners
```

**Modify scaling:**
```bash
kubectl edit hra dotnet-starter-kit-runner-autoscaler -n actions-runner-system
```

### Runner Resources

Each runner has:
- **CPU**: 1-2 cores
- **Memory**: 2-4 GB
- **Storage**: Ephemeral (cleared after each job)

**Adjust resources:**
```bash
kubectl edit runnerdeployment dotnet-starter-kit-runner -n actions-runner-system
```

### Runner Labels

Default labels:
- `self-hosted`
- `linux`
- `x64`
- `gke`

**Add custom labels:**
Edit `github-runner-deployment.yaml`:
```yaml
spec:
  template:
    spec:
      labels:
        - self-hosted
        - linux
        - x64
        - gke
        - dotnet9    # Custom label
        - gpu        # Custom label
```

## Management

### View Runner Status

```powershell
# In Kubernetes
kubectl get runners -n actions-runner-system

# In GitHub
# Go to: https://github.com/Tolli/dotnet-starter-kit/settings/actions/runners
```

### View Logs

```powershell
# Get all runner logs
kubectl logs -l app.kubernetes.io/name=actions-runner -n actions-runner-system --tail=100

# Follow logs in real-time
kubectl logs -l app.kubernetes.io/name=actions-runner -n actions-runner-system -f

# Logs from specific runner
kubectl logs <runner-pod-name> -n actions-runner-system
```

### Manual Scaling

```powershell
# Scale to 5 runners
kubectl scale runnerdeployment dotnet-starter-kit-runner --replicas=5 -n actions-runner-system

# Scale to 0 (pause)
kubectl scale runnerdeployment dotnet-starter-kit-runner --replicas=0 -n actions-runner-system
```

### Restart Runners

```powershell
# Restart all runners
kubectl rollout restart runnerdeployment dotnet-starter-kit-runner -n actions-runner-system

# Delete and recreate
kubectl delete runners --all -n actions-runner-system
# They will automatically recreate
```

## Monitoring

### Check Autoscaler Status

```powershell
kubectl get hra dotnet-starter-kit-runner-autoscaler -n actions-runner-system -o yaml
```

### Check Controller Logs

```powershell
kubectl logs -l app.kubernetes.io/name=actions-runner-controller -n actions-runner-system --tail=100
```

### Metrics

```powershell
# Number of runners
kubectl get runners -n actions-runner-system --no-headers | wc -l

# Runner states
kubectl get runners -n actions-runner-system -o jsonpath='{.items[*].status.phase}'
```

## Troubleshooting

### Runners Not Appearing in GitHub

**Check:**
```powershell
# 1. Check secret exists
kubectl get secret controller-manager -n actions-runner-system

# 2. Check token is valid
kubectl get secret controller-manager -n actions-runner-system -o jsonpath='{.data.github_token}' | base64 -d

# 3. Check controller logs
kubectl logs -l app.kubernetes.io/name=actions-runner-controller -n actions-runner-system --tail=50
```

**Solution:**
```powershell
# Recreate secret with new token
kubectl delete secret controller-manager -n actions-runner-system
kubectl create secret generic controller-manager \
  -n actions-runner-system \
  --from-literal=github_token=YOUR_NEW_TOKEN
  
# Restart controller
kubectl rollout restart deployment actions-runner-controller -n actions-runner-system
```

### Runners Stuck in Pending

**Check:**
```powershell
kubectl describe runner <runner-name> -n actions-runner-system
```

**Common causes:**
- Insufficient cluster resources
- Image pull errors
- Network issues

**Solution:**
```powershell
# Check node resources
kubectl top nodes

# Check pod events
kubectl get events -n actions-runner-system --sort-by='.lastTimestamp'
```

### Workflow Not Using Self-Hosted Runner

**Check:**
1. Verify `runs-on: self-hosted` in workflow
2. Check runner labels match
3. Ensure runner is online in GitHub

**Solution:**
```yaml
# Use specific labels to target your runners
runs-on: [self-hosted, linux, x64, gke]
```

### Docker Build Fails

**Issue:** Docker-in-Docker not working

**Solution:** Use the runner image with Docker:
```yaml
# In github-runner-deployment.yaml
spec:
  template:
    spec:
      image: summerwind/actions-runner-dind:latest
      dockerdWithinRunnerContainer: true
```

## Cost Optimization

### Reduce Costs

1. **Set minimum replicas to 0**
   ```yaml
   minReplicas: 0  # No runners when idle
   ```

2. **Use preemptible nodes** for runner node pool
   ```bash
   gcloud container node-pools create runner-pool \
     --cluster=your-cluster \
     --preemptible \
     --num-nodes=1
   ```

3. **Use smaller machine types**
   ```yaml
   resources:
     requests:
       cpu: "500m"
       memory: "1Gi"
   ```

4. **Scale down during off-hours**
   ```bash
   # Add CronJob to scale down at night
   kubectl create cronjob scale-down \
     --schedule="0 22 * * *" \
     --image=bitnami/kubectl \
     -- kubectl scale runnerdeployment dotnet-starter-kit-runner --replicas=0 -n actions-runner-system
   ```

## Security

### Best Practices

1. ? **Use ephemeral runners** (default) - Fresh runner for each job
2. ? **Rotate GitHub tokens** every 90 days
3. ? **Use namespace isolation**
4. ? **Limit runner permissions** via RBAC
5. ? **Monitor runner logs** for suspicious activity
6. ? **Use private clusters** if possible

### GitHub Token Permissions

Minimize token scope:
- Repository-scoped (not org-wide) if possible
- Read/Write only what's needed
- Set expiration date

### Network Security

```yaml
# Add network policy to restrict runner traffic
apiVersion: networking.k8s.io/v1
kind: NetworkPolicy
metadata:
  name: runner-network-policy
  namespace: actions-runner-system
spec:
  podSelector:
    matchLabels:
      app.kubernetes.io/name: actions-runner
  policyTypes:
  - Egress
  egress:
  - to:
    - podSelector: {}
  - to:
    - namespaceSelector: {}
  - ports:
    - port: 443
      protocol: TCP
    - port: 80
      protocol: TCP
```

## Migration from GitHub-Hosted

### Update Workflows

**Before:**
```yaml
runs-on: ubuntu-latest
```

**After:**
```yaml
runs-on: self-hosted
# Or with specific labels
runs-on: [self-hosted, linux, x64, gke]
```

### Gradual Migration

1. Keep some jobs on GitHub-hosted
2. Move heavy jobs to self-hosted first
3. Monitor performance and costs
4. Migrate remaining jobs

### Example:

```yaml
jobs:
  test-quick:
    runs-on: ubuntu-latest  # Fast, simple tests
    
  test-integration:
    runs-on: self-hosted  # Long-running, heavy tests
    
  build-docker:
    runs-on: self-hosted  # Docker builds benefit most
```

## Uninstallation

### Remove Runners

```powershell
# Delete runner deployment
kubectl delete runnerdeployment dotnet-starter-kit-runner -n actions-runner-system

# Delete autoscaler
kubectl delete hra dotnet-starter-kit-runner-autoscaler -n actions-runner-system

# Uninstall controller
helm uninstall actions-runner-controller -n actions-runner-system

# Delete namespace
kubectl delete namespace actions-runner-system

# Uninstall cert-manager (if not used by others)
kubectl delete -f https://github.com/cert-manager/cert-manager/releases/download/v1.13.0/cert-manager.yaml
```

## Resources

- [Actions Runner Controller Docs](https://github.com/actions/actions-runner-controller)
- [GitHub Self-Hosted Runners](https://docs.github.com/en/actions/hosting-your-own-runners)
- [Kubernetes Autoscaling](https://kubernetes.io/docs/tasks/run-application/horizontal-pod-autoscale/)

## Support

For issues:
1. Check runner logs
2. Check controller logs
3. Review GitHub Actions logs
4. Check Kubernetes events

---

?? **You now have self-hosted GitHub Actions runners in your GKE cluster!**
