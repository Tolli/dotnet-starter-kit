# Self-Hosted GitHub Actions Runner - Quick Reference

## ?? Setup (One-Time)

```powershell
# 1. Update script parameters
# Edit: setup-gke-runner.ps1
# - $ClusterName = "your-cluster-name"
# - $ClusterZone = "europe-west9-a"  
# - $GitHubRepo = "Tolli/dotnet-starter-kit"

# 2. Run setup
cd .github\workflows
.\setup-gke-runner.ps1

# 3. When prompted, enter GitHub PAT
# Create at: https://github.com/settings/tokens
# Scopes: repo, workflow

# 4. Verify
# Check: https://github.com/Tolli/dotnet-starter-kit/settings/actions/runners
```

## ?? Use in Workflows

```yaml
jobs:
  build:
    runs-on: self-hosted
    # OR with labels
    runs-on: [self-hosted, linux, x64, gke]
```

## ?? Common Commands

```powershell
# View runners
kubectl get runners -n actions-runner-system

# View logs
kubectl logs -l app.kubernetes.io/name=actions-runner -n actions-runner-system --tail=50

# Scale manually
kubectl scale runnerdeployment dotnet-starter-kit-runner --replicas=5 -n actions-runner-system

# Restart runners
kubectl rollout restart runnerdeployment dotnet-starter-kit-runner -n actions-runner-system

# Check autoscaler
kubectl get hra -n actions-runner-system

# Delete all runners
kubectl delete runners --all -n actions-runner-system
```

## ?? Quick Fixes

### Runners not showing in GitHub?
```powershell
# Check controller logs
kubectl logs -l app.kubernetes.io/name=actions-runner-controller -n actions-runner-system

# Restart controller
kubectl rollout restart deployment actions-runner-controller -n actions-runner-system
```

### Workflow not using self-hosted?
- Check `runs-on: self-hosted` in workflow
- Verify runner is online (green) in GitHub
- Check labels match

### Need to recreate?
```powershell
# Delete everything
kubectl delete namespace actions-runner-system

# Re-run setup
.\setup-gke-runner.ps1
```

## ?? Cost Calculator

**GitHub-Hosted Cost:**
```
Builds per day × Minutes per build × 30 days × $0.008/min
Example: 200 × 12 × 30 × 0.008 = $576/month
```

**Self-Hosted Cost:**
```
GKE cluster + Nodes + Networking
Example: $70 + $100 + $15 = $185/month
Savings: $391/month (68%)
```

## ?? Performance

| Metric | GitHub-Hosted | Self-Hosted |
|--------|--------------|-------------|
| Startup | 20-60s | 5-10s |
| Docker build | 180-300s | 60-120s |
| Total build | 10-15 min | 5-8 min |

## ?? Quick Links

- **Runners**: https://github.com/Tolli/dotnet-starter-kit/settings/actions/runners
- **GitHub Tokens**: https://github.com/settings/tokens
- **Actions Runner Controller**: https://github.com/actions/actions-runner-controller

## ?? Full Documentation

- [RUNNER-SETUP-SUMMARY.md](RUNNER-SETUP-SUMMARY.md) - Overview
- [SELF-HOSTED-RUNNER-GUIDE.md](SELF-HOSTED-RUNNER-GUIDE.md) - Complete guide
- [RUNNER-COMPARISON.md](RUNNER-COMPARISON.md) - GitHub-hosted vs Self-hosted

---

**Need more help?** Check the full documentation files above.
