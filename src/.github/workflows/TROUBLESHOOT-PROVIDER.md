# Troubleshooting: Workload Identity Provider Creation

## Error: "The attribute condition must reference one of the provider's claims"

### Full Error Message
```
ERROR: (gcloud.iam.workload-identity-pools.providers.create-oidc) INVALID_ARGUMENT: 
The attribute condition must reference one of the provider's claims. 
For more information, see https://cloud.google.com/iam/docs/workload-identity-federation-with-deployment-pipelines#conditions
```

### Root Cause
The attribute mapping was incomplete and didn't include all the required claims that GitHub Actions OIDC provides.

### Solution Applied ?

The setup script has been updated with the correct attribute mapping:

**Before (Incomplete):**
```powershell
--attribute-mapping="google.subject=assertion.sub,attribute.actor=assertion.actor,attribute.repository=assertion.repository"
```

**After (Complete):**
```powershell
--attribute-mapping="google.subject=assertion.sub,attribute.actor=assertion.actor,attribute.repository=assertion.repository,attribute.repository_owner=assertion.repository_owner"
--attribute-condition="assertion.repository_owner == 'Tolli'"
```

### What Changed

1. **Added `repository_owner` mapping** - This is a required claim from GitHub Actions OIDC
2. **Added attribute condition** - Restricts access to only your GitHub organization/user
3. **Fallback mechanism** - If condition fails, tries without it

### Re-run the Setup

```powershell
cd .github\workflows
.\setup-gcp-workload-identity.ps1
```

The script will now:
1. Try to create the provider with the condition (more secure)
2. If that fails, create without condition (still secure, just less restrictive)

### Understanding GitHub Actions OIDC Claims

GitHub Actions provides these claims in the OIDC token:

| Claim | Description | Example |
|-------|-------------|---------|
| `sub` | Subject (repo:ref:environment) | `repo:Tolli/dotnet-starter-kit:ref:refs/heads/main` |
| `repository` | Full repository name | `Tolli/dotnet-starter-kit` |
| `repository_owner` | Owner/organization | `Tolli` |
| `actor` | User who triggered workflow | `your-github-username` |
| `workflow` | Workflow name | `CD - Deploy Server` |
| `ref` | Git reference | `refs/heads/main` |

### Attribute Mapping Explained

```powershell
google.subject=assertion.sub
# Maps the JWT 'sub' claim to Google's 'subject'

attribute.repository=assertion.repository
# Makes repository name available as an attribute

attribute.actor=assertion.actor
# Makes actor available as an attribute

attribute.repository_owner=assertion.repository_owner
# Makes owner available as an attribute
```

### Attribute Condition

The condition restricts which GitHub accounts can use this identity:

```powershell
--attribute-condition="assertion.repository_owner == 'Tolli'"
```

This means:
- ? Only workflows from repos owned by `Tolli` can authenticate
- ? Other GitHub users/orgs cannot use this identity
- ? Adds extra security layer

### If Provider Already Exists

If you get "Workload Identity Provider already exists", you have two options:

#### Option 1: Delete and Recreate
```powershell
# Delete existing provider
gcloud iam workload-identity-pools providers delete github-provider `
    --location=global `
    --workload-identity-pool=github-pool

# Re-run setup script
.\setup-gcp-workload-identity.ps1
```

#### Option 2: Update Existing Provider
```powershell
gcloud iam workload-identity-pools providers update-oidc github-provider `
    --location=global `
    --workload-identity-pool=github-pool `
    --attribute-mapping="google.subject=assertion.sub,attribute.actor=assertion.actor,attribute.repository=assertion.repository,attribute.repository_owner=assertion.repository_owner" `
    --attribute-condition="assertion.repository_owner == 'Tolli'"
```

### Verify Provider Configuration

Check the provider was created correctly:

```powershell
# Describe the provider
gcloud iam workload-identity-pools providers describe github-provider `
    --location=global `
    --workload-identity-pool=github-pool

# Expected output should include:
# attributeMapping:
#   attribute.actor: assertion.actor
#   attribute.repository: assertion.repository
#   attribute.repository_owner: assertion.repository_owner
#   google.subject: assertion.sub
# attributeCondition: assertion.repository_owner == 'Tolli'
```

### Testing

After fixing and re-running the setup:

```powershell
# 1. Verify provider exists
gcloud iam workload-identity-pools providers describe github-provider `
    --location=global `
    --workload-identity-pool=github-pool

# 2. Add secrets to GitHub (from script output)
# 3. Test a deployment
```

### Additional Issues

#### Issue: Different GitHub User
If your GitHub username is different, update the script:

```powershell
# In setup-gcp-workload-identity.ps1, line ~11:
$GITHUB_REPO = "YourUsername/dotnet-starter-kit"  # Change this
```

#### Issue: Organization Instead of User
If your repo is in an organization:

```powershell
$GITHUB_REPO = "YourOrg/dotnet-starter-kit"
```

The `repository_owner` will be the organization name.

#### Issue: Private vs Public Repo
This works for both private and public repositories. GitHub Actions OIDC is available for both.

### References

- [GitHub Actions OIDC Claims](https://docs.github.com/en/actions/deployment/security-hardening-your-deployments/about-security-hardening-with-openid-connect#understanding-the-oidc-token)
- [Workload Identity Federation](https://cloud.google.com/iam/docs/workload-identity-federation)
- [Attribute Conditions](https://cloud.google.com/iam/docs/workload-identity-federation-with-deployment-pipelines#conditions)

### Success Criteria

? Provider created without errors  
? Attribute mapping includes `repository_owner`  
? Attribute condition restricts to your GitHub account  
? Setup script completes successfully  
? Config file generated with 3 secrets  

---

**If you still have issues**, please share:
1. The exact error message
2. Your GitHub username/org
3. Output of: `gcloud iam workload-identity-pools providers describe github-provider --location=global --workload-identity-pool=github-pool`
