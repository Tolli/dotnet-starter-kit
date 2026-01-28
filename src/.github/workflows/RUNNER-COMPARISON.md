# GitHub-Hosted vs Self-Hosted Runners Comparison

## Overview

This guide helps you decide when to use GitHub-hosted runners vs self-hosted runners in GKE.

## Quick Comparison

| Feature | GitHub-Hosted | Self-Hosted (GKE) |
|---------|--------------|-------------------|
| **Setup** | ? None | ?? Initial setup required |
| **Cost** | ?? Pay per minute | ?? GKE cluster costs |
| **Speed** | ?? Cold start each time | ? Faster (persistent cache) |
| **Private Access** | ? No | ? Yes (VPC, internal services) |
| **Customization** | ?? Limited | ? Full control |
| **Maintenance** | ? None | ?? You manage |
| **Scaling** | ? Automatic | ? Auto-scale (ARC) |
| **Security** | ? Isolated | ?? You secure |

## Cost Analysis

### GitHub-Hosted Runners

**Pricing (as of 2024):**
- Linux: **$0.008/minute**
- Windows: **$0.016/minute**
- macOS: **$0.08/minute**

**Free tier:**
- Public repos: Unlimited
- Private repos: 2,000 minutes/month (Pro), 3,000 (Team)

**Example Monthly Cost:**
- 100 builds/day × 10 min/build × 30 days = 30,000 minutes
- Cost: 30,000 × $0.008 = **$240/month**

### Self-Hosted in GKE

**Components:**
- GKE cluster management: ~$70/month
- Node costs (n1-standard-2): ~$50/node/month
- Networking: ~$10/month
- Storage: ~$5/month

**Example Monthly Cost:**
- Cluster + 2 nodes + networking + storage = **~$185/month**
- **Savings: $55/month** (23% less)

**Break-even point:**
- Self-hosted becomes cheaper at ~2,300 build minutes/month
- ~75 builds/day at 10 min each

## Performance Comparison

### Build Speed

| Stage | GitHub-Hosted | Self-Hosted (GKE) |
|-------|--------------|-------------------|
| **Startup** | 20-60s | 5-10s |
| **Checkout** | 10-30s | 5-10s |
| **Restore** | 60-120s | 10-30s (cached) |
| **Build** | 120-180s | 120-180s (same) |
| **Test** | 60-90s | 60-90s (same) |
| **Docker Build** | 180-300s | 60-120s (cached) |
| **Push Image** | 60-120s | 10-30s (same network) |
| **Total** | ~10-15 min | ~5-8 min |

**Improvement: 40-50% faster** with self-hosted

### Why Self-Hosted is Faster

1. **Persistent Docker cache**
   - GitHub-hosted: Fresh every time
   - Self-hosted: Cached layers

2. **Network proximity**
   - GitHub-hosted: External network
   - Self-hosted: Same VPC as GKE/Artifact Registry

3. **Pre-installed tools**
   - GitHub-hosted: Limited tools
   - Self-hosted: Pre-install what you need

4. **No cold starts**
   - GitHub-hosted: New VM each time
   - Self-hosted: Pod stays warm

## When to Use Each

### Use GitHub-Hosted When:

? **Low build frequency** (< 2,000 minutes/month)
- Free tier covers your needs
- Don't want infrastructure overhead

? **Public open-source projects**
- Unlimited free minutes
- No setup required

? **Simple builds**
- Standard tools (Node, Python, Go)
- No private resource access needed
- No Docker caching benefits

? **Multiple OS needed**
- Need Windows or macOS runners
- Testing cross-platform

? **Security critical**
- Don't want to manage runner security
- Prefer GitHub's isolated environment

### Use Self-Hosted (GKE) When:

? **High build frequency** (> 2,300 minutes/month)
- Cost savings add up
- Better performance ROI

? **Docker-heavy workflows**
- Building many Docker images
- Benefit from layer caching
- Avoid Docker Hub rate limits

? **Private resource access**
- Internal APIs
- Database connections
- VPC services
- Private artifact registries

? **Custom requirements**
- Specific tools or versions
- GPU acceleration
- Large memory/CPU needs

? **Monorepo builds**
- Large codebases
- Long build times
- Benefit from caching

## Migration Strategy

### Phase 1: Assessment (Week 1)

1. **Analyze current usage**
   ```bash
   # Check GitHub Actions usage
   # Settings ? Billing ? Actions minutes
   ```

2. **Identify bottlenecks**
   - Slow workflows
   - Frequent runs
   - Docker builds
   - Large downloads

3. **Calculate costs**
   - Current GitHub Actions costs
   - Estimated GKE costs
   - Break-even analysis

### Phase 2: Setup (Week 2)

1. **Deploy self-hosted runners**
   ```powershell
   .\setup-gke-runner.ps1
   ```

2. **Configure autoscaling**
   - Set min/max replicas
   - Test scaling behavior

3. **Verify connectivity**
   - Can access private resources?
   - Network policies OK?

### Phase 3: Pilot (Week 3-4)

1. **Migrate one workflow**
   ```yaml
   # Start with Docker build workflow
   jobs:
     build-docker:
       runs-on: self-hosted
   ```

2. **Monitor performance**
   - Build times
   - Success rate
   - Resource usage

3. **Gather feedback**
   - Team experience
   - Issues encountered
   - Performance gains

### Phase 4: Full Migration (Week 5-6)

1. **Migrate remaining workflows**
   ```yaml
   # Update all workflows
   runs-on: self-hosted
   ```

2. **Keep fallback**
   ```yaml
   # Use matrix for redundancy
   strategy:
     matrix:
       runner: [self-hosted, ubuntu-latest]
   ```

3. **Monitor costs**
   - GKE cluster costs
   - Reduced GitHub Actions costs
   - Net savings

## Hybrid Approach (Recommended)

Use **both** GitHub-hosted and self-hosted:

```yaml
jobs:
  # Fast, simple checks on GitHub-hosted
  lint:
    runs-on: ubuntu-latest
    steps:
      - run: npm run lint
  
  # Heavy builds on self-hosted
  build-and-test:
    runs-on: self-hosted
    steps:
      - run: dotnet build
      - run: dotnet test
  
  # Docker builds on self-hosted (benefits most)
  build-docker:
    runs-on: self-hosted
    steps:
      - run: docker build
      - run: docker push
  
  # Deploy on GitHub-hosted (security)
  deploy:
    runs-on: ubuntu-latest
    needs: [build-and-test, build-docker]
    steps:
      - run: kubectl apply -f k8s/
```

## Real-World Examples

### Example 1: Small Team (5 developers)

**Before (GitHub-Hosted):**
- 50 builds/day × 8 min/build × 22 days = 8,800 minutes/month
- Cost: $70/month (exceeds free tier)

**After (Self-Hosted):**
- GKE cluster (1 node): ~$135/month
- **Net cost: +$65/month**
- ? **Not worth it** - too few builds

**Recommendation:** Stay on GitHub-hosted

### Example 2: Medium Team (15 developers)

**Before (GitHub-Hosted):**
- 200 builds/day × 12 min/build × 22 days = 52,800 minutes/month
- Cost: $422/month

**After (Self-Hosted):**
- GKE cluster (2 nodes): ~$185/month
- **Savings: $237/month (56%)**
- Build time reduced from 12 min ? 7 min

**Recommendation:** ? Self-hosted is worth it

### Example 3: Large Team (50 developers)

**Before (GitHub-Hosted):**
- 800 builds/day × 15 min/build × 22 days = 264,000 minutes/month
- Cost: $2,112/month

**After (Self-Hosted):**
- GKE cluster (5 nodes with autoscaling): ~$350/month
- **Savings: $1,762/month (83%)**
- Build time reduced from 15 min ? 8 min

**Recommendation:** ?? Definitely self-hosted

## Decision Flowchart

```
Start
  |
  ?? Build frequency < 2,000 min/month?
  |  ?? Yes ? Use GitHub-Hosted ?
  |  ?? No ? Continue
  |
  ?? Need private resource access?
  |  ?? Yes ? Use Self-Hosted ?
  |  ?? No ? Continue
  |
  ?? Many Docker builds?
  |  ?? Yes ? Use Self-Hosted ?
  |  ?? No ? Continue
  |
  ?? Build time > 10 min?
  |  ?? Yes ? Use Self-Hosted ?
  |  ?? No ? Continue
  |
  ?? Have GKE cluster already?
  |  ?? Yes ? Use Self-Hosted ?
  |  ?? No ? Continue
  |
  ?? Default ? Use GitHub-Hosted ?
```

## Summary

### Use GitHub-Hosted If:
- ?? Small team (< 10 developers)
- ?? Low build frequency
- ?? Simple workflows
- ?? Security is top priority
- ?? Budget for GitHub Actions minutes

### Use Self-Hosted If:
- ?? Medium-large team (> 15 developers)
- ?? High build frequency (> 2,300 min/month)
- ?? Docker-heavy workflows
- ?? Need private resource access
- ? Want faster build times
- ?? Want to reduce costs long-term

### Hybrid Approach:
- ? Best of both worlds
- ?? Use right runner for each job
- ?? Optimize costs
- ? Maximize performance

---

**Need help deciding?** Calculate your usage:
1. Check Settings ? Billing ? Actions minutes
2. Multiply by $0.008/minute
3. Compare to ~$185/month for self-hosted
4. Factor in build time improvements
