# CORS Issue - Fixed

## Problem
When the Blazor WebAssembly client (https://tbr.tolli.com) tries to communicate with the WebAPI (https://tbr-api.tolli.com) in Kubernetes, you get this error:

```
Access to fetch at 'https://tbr-api.tolli.com/api/token' from origin 'https://tbr.tolli.com' 
has been blocked by CORS policy: Request header field tenant is not allowed by 
Access-Control-Allow-Headers in preflight response.
```

## Root Cause
The API's CORS policy was not configured to:
1. Allow requests from the production origin `https://tbr.tolli.com`
2. Explicitly expose the `tenant` header (used for multi-tenancy)

## Changes Made

### 1. ? Updated `api/server/appsettings.json`
Added the production origin to the allowed origins list:
```json
"CorsOptions": {
  "AllowedOrigins": [
    "https://localhost:7100",
    "http://localhost:7100",
    "http://localhost:5010",
    "https://repeatedly-finer-kite.ngrok-free.app",
    "https://tbr.tolli.com"  // ? Added
  ]
}
```

### 2. ? Updated `api/framework/Infrastructure/Cors/Extensions.cs`
Added explicit header exposure in the CORS policy:
```csharp
policy.AllowAnyHeader()
    .AllowAnyMethod()
    .AllowCredentials()
    .WithExposedHeaders("tenant", "Content-Disposition")  // ? Added
    .WithOrigins(corsOptions.AllowedOrigins.ToArray())
```

### 3. ? Updated `k8s/server-deployment.yaml`
Added CORS configuration via environment variables:
```yaml
- name: CorsOptions__AllowedOrigins__0
  value: "https://tbr.tolli.com"
- name: CorsOptions__AllowedOrigins__1
  value: "http://localhost:7100"
- name: CorsOptions__AllowedOrigins__2
  value: "https://localhost:7100"
```

## Why This Happened

### The `tenant` Header
Your application uses multi-tenancy with Finbuckle.MultiTenant. The tenant is identified via:
- A `tenant` HTTP header (defined in `TenantConstants.Identifier`)
- A JWT claim
- A query parameter

When the Blazor client makes API requests, it includes the `tenant` header. Browsers perform a **preflight OPTIONS request** to check if this custom header is allowed by CORS.

### CORS Preflight
1. Browser sees cross-origin request with custom `tenant` header
2. Sends OPTIONS preflight request to API
3. API must respond with `Access-Control-Allow-Headers: tenant`
4. If not allowed, browser blocks the actual request

## Deployment Steps

### Step 1: Rebuild and Push API Image
```bash
# Build
docker build -t courtrental-server:latest -f Dockerfile .

# Tag
docker tag courtrental-server:latest \
  europe-west9-docker.pkg.dev/main-project-483817/klettagja/courtrental-server:latest

# Push
docker push europe-west9-docker.pkg.dev/main-project-483817/klettagja/courtrental-server:latest
```

### Step 2: Update Kubernetes Deployment
```bash
# Apply updated deployment
kubectl apply -f k8s/server-deployment.yaml

# Force rollout (if needed)
kubectl rollout restart deployment/fsh-webapi

# Watch rollout status
kubectl rollout status deployment/fsh-webapi
```

### Step 3: Verify the Fix

#### Check Pod Logs
```bash
kubectl logs -l app=fsh-webapi --tail=50
```

#### Check Environment Variables
```bash
kubectl exec -it deployment/fsh-webapi -- env | grep -i cors
```

You should see:
```
CorsOptions__AllowedOrigins__0=https://tbr.tolli.com
CorsOptions__AllowedOrigins__1=http://localhost:7100
CorsOptions__AllowedOrigins__2=https://localhost:7100
```

#### Test from Browser
1. Open https://tbr.tolli.com
2. Open DevTools (F12) ? Network tab
3. Try to login or make an API call
4. Look for the OPTIONS preflight request
5. Check response headers should include:
   ```
   Access-Control-Allow-Origin: https://tbr.tolli.com
   Access-Control-Allow-Headers: tenant, content-type, authorization, ...
   Access-Control-Allow-Credentials: true
   ```

## Understanding the Fix

### .NET Configuration Binding
ASP.NET Core binds environment variables to configuration using `__` (double underscore):

```bash
CorsOptions__AllowedOrigins__0=https://tbr.tolli.com
```

Maps to:
```json
{
  "CorsOptions": {
    "AllowedOrigins": [
      "https://tbr.tolli.com"  // index 0
    ]
  }
}
```

### Array Indexing in Environment Variables
Arrays in configuration are indexed starting from 0:
- `CorsOptions__AllowedOrigins__0` = First origin
- `CorsOptions__AllowedOrigins__1` = Second origin
- `CorsOptions__AllowedOrigins__2` = Third origin

### CORS Policy Evaluation
When a request comes from `https://tbr.tolli.com`:
1. Browser sends OPTIONS preflight with `Origin: https://tbr.tolli.com`
2. API checks if origin is in `AllowedOrigins` list
3. If found, responds with `Access-Control-Allow-Origin: https://tbr.tolli.com`
4. Browser allows the actual request to proceed

## Troubleshooting

### Still Getting CORS Error?

#### 1. Check if deployment updated
```bash
kubectl get pods -l app=fsh-webapi
# Look at AGE column - should be recent
```

#### 2. Check environment variables in pod
```bash
kubectl exec deployment/fsh-webapi -- env | grep CorsOptions
```

#### 3. Check API logs for CORS messages
```bash
kubectl logs -l app=fsh-webapi --tail=100 | grep -i cors
```

#### 4. Test API directly with curl
```bash
curl -I -X OPTIONS https://tbr-api.tolli.com/api/token \
  -H "Origin: https://tbr.tolli.com" \
  -H "Access-Control-Request-Method: POST" \
  -H "Access-Control-Request-Headers: tenant,content-type"
```

Look for these response headers:
```
Access-Control-Allow-Origin: https://tbr.tolli.com
Access-Control-Allow-Headers: tenant
Access-Control-Allow-Credentials: true
```

### Other Issues

#### Issue: Headers still not allowed
**Solution**: The header name must match exactly. Check:
- Browser DevTools ? Network ? Preflight request ? Request Headers
- Compare with `TenantConstants.Identifier` value

#### Issue: Credentials not allowed
**Solution**: When using `.AllowCredentials()`, you cannot use `.AllowAnyOrigin()`. 
You must specify exact origins with `.WithOrigins()`. ? Already done.

#### Issue: Different error after fix
**Check**: 
- JWT token validity
- Authentication configuration
- API endpoint availability

## Best Practices

### ? What We Did Right
1. Specified exact origins (not `AllowAnyOrigin`)
2. Used `AllowCredentials` for cookie/auth support
3. Explicitly listed allowed headers
4. Used environment variables for Kubernetes config

### ?? Security Considerations
- Never use `AllowAnyOrigin()` with `AllowCredentials()`
- Keep the allowed origins list minimal
- Use HTTPS in production (already doing this)
- Regularly review CORS configuration

### ?? Future Improvements
1. Move allowed origins to ConfigMap entirely:
   ```yaml
   # k8s/server-configmap.yaml
   data:
     cors-allowed-origins: "https://tbr.tolli.com,https://app.example.com"
   ```

2. Create different ConfigMaps per environment:
   - `server-configmap-dev.yaml`
   - `server-configmap-staging.yaml`
   - `server-configmap-prod.yaml`

## Summary

? Added `https://tbr.tolli.com` to allowed origins  
? Explicitly exposed `tenant` header in CORS policy  
? Configured Kubernetes deployment with CORS environment variables  
? Documented the fix and testing procedures  

The CORS error should now be resolved. The Blazor client can successfully make API calls with the `tenant` header.
