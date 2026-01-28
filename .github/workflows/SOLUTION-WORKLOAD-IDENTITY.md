# ? SOLUTION: Service Account Key Creation Blocked

## Problem
```
ERROR: Key creation is not allowed on this service account.
constraints/iam.disableServiceAccountKeyCreation
```

## Root Cause
Your GCP organization has a **security policy** that prevents creating service account keys. This is actually a **GOOD thing** - it's a security best practice!

## Solution: Workload Identity Federation ?

Instead of using service account keys (which can be stolen), use **Workload Identity Federation** - a more secure method that uses short-lived tokens.

---

## ?? Quick Fix (10 Minutes)

### 1. Run the New Setup Script
```powershell
cd .github\workflows
.\setup-gcp-workload-identity.ps1
```

### 2. Add 3 Secrets to GitHub
Go to: https://github.com/Tolli/dotnet-starter-kit/settings/secrets/actions

Add these secrets (values from script output):
- `GCP_PROJECT_ID`
- `GCP_SERVICE_ACCOUNT`  
- `GCP_WORKLOAD_IDENTITY_PROVIDER`

### 3. Commit Updated Workflows
```bash
git add .github/workflows/
git commit -m "Switch to Workload Identity Federation"
git push origin main
```

### 4. Test Deployment
Go to **Actions** ? **CD - Deploy Server** ? **Run workflow**

---

## ? What Changed?

### Before (Insecure):
```yaml
- name: Authenticate to Google Cloud
  uses: google-github-actions/auth@v2
  with:
    credentials_json: ${{ secrets.GCP_SA_KEY }}  # ? Static key
```

### After (Secure):
```yaml
- name: Authenticate to Google Cloud
  uses: google-github-actions/auth@v2
  with:
    workload_identity_provider: ${{ secrets.GCP_WORKLOAD_IDENTITY_PROVIDER }}
    service_account: ${{ secrets.GCP_SERVICE_ACCOUNT }}
    # ? No keys! Uses short-lived tokens
```

---

## ?? Benefits

| Before (Keys) | After (Workload Identity) |
|--------------|--------------------------|
| ? Static keys that can leak | ? Short-lived tokens (expire in 1 hour) |
| ? Manual rotation needed | ? Automatic rotation |
| ? Blocked by org policy | ? Compliant with security policies |
| ? Security risk | ? Best practice |
| ? Hard to audit | ? Full audit trail |

---

## ?? Files Created/Updated

### New Files:
- ? `.github/workflows/setup-gcp-workload-identity.ps1` - Setup script
- ? `.github/workflows/WORKLOAD-IDENTITY-GUIDE.md` - Complete guide

### Updated Files:
- ? `.github/workflows/deploy-server.yml` - Now uses Workload Identity
- ? `.github/workflows/deploy-client.yml` - Now uses Workload Identity

### Old Files (No longer needed):
- ?? `.github/workflows/setup-gcp.ps1` - Old key-based setup
- ?? `.github/workflows/setup-gcp.sh` - Old key-based setup

---

## ?? How to Verify It's Working

### 1. Check Workload Identity Pool
```powershell
gcloud iam workload-identity-pools describe github-pool --location=global
```

### 2. Check Provider
```powershell
gcloud iam workload-identity-pools providers describe github-provider `
  --location=global `
  --workload-identity-pool=github-pool
```

### 3. Test GitHub Action
1. Go to **Actions** tab
2. Run any deployment workflow
3. Check "Authenticate to Google Cloud" step
4. Should succeed without key errors

---

## ? FAQ

**Q: Do I need to delete the old `GCP_SA_KEY` secret?**  
A: Yes, you can delete it since it's not used anymore (and probably doesn't work anyway).

**Q: Is this more complicated?**  
A: Setup is slightly more complex, but much more secure and requires less maintenance.

**Q: What if the setup script fails?**  
A: Check the [WORKLOAD-IDENTITY-GUIDE.md](WORKLOAD-IDENTITY-GUIDE.md) troubleshooting section.

**Q: Can I use this for other CI/CD systems?**  
A: Yes! Workload Identity Federation works with GitHub Actions, GitLab CI, CircleCI, and more.

**Q: What if I don't have permission to create Workload Identity Pools?**  
A: Ask your GCP admin to grant you `roles/iam.workloadIdentityPoolAdmin` role.

---

## ?? Documentation

- **Quick Setup**: Run `.\setup-gcp-workload-identity.ps1`
- **Complete Guide**: [WORKLOAD-IDENTITY-GUIDE.md](WORKLOAD-IDENTITY-GUIDE.md)
- **Testing**: [TESTING.md](TESTING.md) - Update with new secrets
- **Main README**: [README.md](README.md) - Full reference

---

## ?? Success!

Once you complete the setup:
- ? No more service account key errors
- ? More secure authentication
- ? Compliant with organization policies
- ? Automated token rotation
- ? Better audit logs

**Your deployments will work the same way, just more securely!** ??
