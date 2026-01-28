param(
    [string]$Namespace = "tbr",
    [string]$Deployment = "fsh-webapi",
    [string]$ApiUrl = "https://tbr-api.tolli.com",
    [string]$Origin = "https://tbr.tolli.com"
)

Write-Host "=========================================" -ForegroundColor Cyan
Write-Host "🔍 CORS Diagnostic Tool" -ForegroundColor Cyan
Write-Host "=========================================" -ForegroundColor Cyan
Write-Host "Namespace: $Namespace"
Write-Host "Deployment: $Deployment"
Write-Host "API URL: $ApiUrl"
Write-Host "Origin: $Origin"
Write-Host "=========================================" -ForegroundColor Cyan
Write-Host ""

# Step 1: Check if deployment exists
Write-Host "1️⃣  Checking if deployment exists..." -ForegroundColor Yellow
try {
    $null = kubectl get deployment $Deployment -n $Namespace 2>&1
    if ($LASTEXITCODE -ne 0) {
        Write-Host "❌ Deployment $Deployment not found in namespace $Namespace" -ForegroundColor Red
        exit 1
    }
    Write-Host "✅ Deployment found" -ForegroundColor Green
}
catch {
    Write-Host "❌ Error checking deployment: $_" -ForegroundColor Red
    exit 1
}
Write-Host ""

# Step 2: Check pod status
Write-Host "2️⃣  Checking pod status..." -ForegroundColor Yellow
kubectl get pods -l app=$Deployment -n $Namespace
Write-Host ""

# Step 3: Get pod name
$Pod = kubectl get pods -l app=$Deployment -n $Namespace -o jsonpath='{.items[0].metadata.name}' 2>$null
if ([string]::IsNullOrEmpty($Pod)) {
    Write-Host "❌ No running pods found" -ForegroundColor Red
    exit 1
}
Write-Host "Using pod: $Pod" -ForegroundColor Cyan
Write-Host ""

# Step 4: Check image version
Write-Host "3️⃣  Checking Docker image..." -ForegroundColor Yellow
$Image = kubectl get pod $Pod -n $Namespace -o jsonpath='{.spec.containers[0].image}'
Write-Host "Current image: $Image" -ForegroundColor White
Write-Host ""

# Step 5: Check when pod was created
Write-Host "4️⃣  Checking pod age..." -ForegroundColor Yellow
$CreationTimestamp = kubectl get pod $Pod -n $Namespace -o jsonpath='{.metadata.creationTimestamp}'
Write-Host "Pod created: $CreationTimestamp" -ForegroundColor White
Write-Host ""

# Step 6: Check CORS environment variables
Write-Host "5️⃣  Checking CORS environment variables in pod..." -ForegroundColor Yellow
$CorsEnv = kubectl exec $Pod -n $Namespace -- env 2>$null | Select-String -Pattern "cors" -CaseSensitive:$false
if ($CorsEnv) {
    $CorsEnv
} else {
    Write-Host "No CORS environment variables found" -ForegroundColor Yellow
}
Write-Host ""

# Step 7: Check all CorsOptions environment variables
Write-Host "6️⃣  Checking all CorsOptions environment variables..." -ForegroundColor Yellow
$CorsOptions = kubectl exec $Pod -n $Namespace -- env 2>$null | Select-String -Pattern "CorsOptions"
if ($CorsOptions) {
    $CorsOptions
} else {
    Write-Host "No CorsOptions environment variables found" -ForegroundColor Yellow
}
Write-Host ""

# Step 8: Check pod logs for CORS-related messages
Write-Host "7️⃣  Checking pod logs for CORS messages..." -ForegroundColor Yellow
$CorsLogs = kubectl logs $Pod -n $Namespace --tail=100 2>$null | Select-String -Pattern "cors" -CaseSensitive:$false
if ($CorsLogs) {
    $CorsLogs
} else {
    Write-Host "No CORS messages in logs" -ForegroundColor Yellow
}
Write-Host ""

# Step 9: Test actual CORS with HTTP request
Write-Host "8️⃣  Testing CORS with preflight request..." -ForegroundColor Yellow
Write-Host "Sending OPTIONS request to: $ApiUrl/api/token" -ForegroundColor White
Write-Host "From origin: $Origin" -ForegroundColor White
Write-Host ""

try {
    $Headers = @{
        "Origin" = $Origin
        "Access-Control-Request-Method" = "POST"
        "Access-Control-Request-Headers" = "tenant,content-type,authorization"
    }
    
    # Use curl if available, otherwise Invoke-WebRequest
    if (Get-Command curl.exe -ErrorAction SilentlyContinue) {
        Write-Host "Using curl for preflight request..." -ForegroundColor Gray
        $Response = curl.exe -v -X OPTIONS "$ApiUrl/api/token" `
            -H "Origin: $Origin" `
            -H "Access-Control-Request-Method: POST" `
            -H "Access-Control-Request-Headers: tenant,content-type,authorization" 2>&1
        
        $ResponseText = $Response -join "`n"
        Write-Host $ResponseText -ForegroundColor Gray
    } else {
        Write-Host "Using Invoke-WebRequest for preflight request..." -ForegroundColor Gray
        try {
            $Response = Invoke-WebRequest -Uri "$ApiUrl/api/token" -Method Options -Headers $Headers -UseBasicParsing -ErrorAction Stop
            $ResponseText = $Response.Headers | Out-String
        } catch {
            $Response = $_.Exception.Response
            $ResponseText = "Status: $($Response.StatusCode)"
            if ($Response.Headers) {
                $ResponseText += "`n" + ($Response.Headers | Out-String)
            }
        }
        Write-Host $ResponseText -ForegroundColor Gray
    }
} catch {
    Write-Host "Error during preflight request: $_" -ForegroundColor Red
    $ResponseText = ""
}
Write-Host ""

# Step 10: Parse and check CORS headers
Write-Host "9️⃣  Analyzing CORS response headers..." -ForegroundColor Yellow
Write-Host ""

$HasOrigin = $ResponseText -match "Access-Control-Allow-Origin:.*$Origin"
$HasTenant = $ResponseText -match "Access-Control-Allow-Headers:.*tenant"
$HasCredentials = $ResponseText -match "Access-Control-Allow-Credentials:.*true"
$HasPost = $ResponseText -match "Access-Control-Allow-Methods:.*POST"

if ($HasOrigin) {
    Write-Host "✅ Access-Control-Allow-Origin: $Origin is present" -ForegroundColor Green
} else {
    Write-Host "❌ Access-Control-Allow-Origin: $Origin is MISSING" -ForegroundColor Red
    $OriginHeader = ($ResponseText -split "`n" | Select-String -Pattern "Access-Control-Allow-Origin:") -join ""
    if ($OriginHeader) {
        Write-Host "   Found: $OriginHeader" -ForegroundColor Yellow
    } else {
        Write-Host "   Found: Not found" -ForegroundColor Yellow
    }
}

if ($HasTenant) {
    Write-Host "✅ Access-Control-Allow-Headers includes 'tenant'" -ForegroundColor Green
} else {
    Write-Host "❌ Access-Control-Allow-Headers does NOT include 'tenant'" -ForegroundColor Red
    Write-Host "   This is the ROOT CAUSE of your problem!" -ForegroundColor Red -BackgroundColor Yellow
    $HeadersHeader = ($ResponseText -split "`n" | Select-String -Pattern "Access-Control-Allow-Headers:") -join ""
    if ($HeadersHeader) {
        Write-Host "   Found: $HeadersHeader" -ForegroundColor Yellow
    } else {
        Write-Host "   Found: Not found" -ForegroundColor Yellow
    }
}

if ($HasCredentials) {
    Write-Host "✅ Access-Control-Allow-Credentials: true is present" -ForegroundColor Green
} else {
    Write-Host "❌ Access-Control-Allow-Credentials is MISSING or false" -ForegroundColor Red
    $CredHeader = ($ResponseText -split "`n" | Select-String -Pattern "Access-Control-Allow-Credentials:") -join ""
    if ($CredHeader) {
        Write-Host "   Found: $CredHeader" -ForegroundColor Yellow
    } else {
        Write-Host "   Found: Not found" -ForegroundColor Yellow
    }
}

if ($HasPost) {
    Write-Host "✅ Access-Control-Allow-Methods includes 'POST'" -ForegroundColor Green
} else {
    Write-Host "❌ Access-Control-Allow-Methods does NOT include 'POST'" -ForegroundColor Red
    $MethodsHeader = ($ResponseText -split "`n" | Select-String -Pattern "Access-Control-Allow-Methods:") -join ""
    if ($MethodsHeader) {
        Write-Host "   Found: $MethodsHeader" -ForegroundColor Yellow
    } else {
        Write-Host "   Found: Not found" -ForegroundColor Yellow
    }
}

Write-Host ""
Write-Host "🔟 All CORS-related response headers:" -ForegroundColor Yellow
$CorsHeaders = $ResponseText -split "`n" | Select-String -Pattern "access-control" -CaseSensitive:$false
if ($CorsHeaders) {
    $CorsHeaders | ForEach-Object { Write-Host $_ -ForegroundColor White }
} else {
    Write-Host "No CORS headers found in response" -ForegroundColor Yellow
}
Write-Host ""

# Step 11: Summary
Write-Host "=========================================" -ForegroundColor Cyan
Write-Host "📊 Summary" -ForegroundColor Cyan
Write-Host "=========================================" -ForegroundColor Cyan
Write-Host ""

if ($HasTenant) {
    Write-Host "✅ CORS is configured correctly!" -ForegroundColor Green
    Write-Host "   If you're still seeing errors in the browser:" -ForegroundColor Yellow
    Write-Host "   1. Clear browser cache" -ForegroundColor Yellow
    Write-Host "   2. Try incognito/private mode" -ForegroundColor Yellow
    Write-Host "   3. Check browser DevTools console for other errors" -ForegroundColor Yellow
} else {
    Write-Host "❌ CORS is NOT configured correctly!" -ForegroundColor Red
    Write-Host ""
    Write-Host "The API is not allowing the 'tenant' header." -ForegroundColor Yellow
    Write-Host ""
    Write-Host "Possible causes:" -ForegroundColor Yellow
    Write-Host "1. The Docker image hasn't been rebuilt with the new CORS code" -ForegroundColor White
    Write-Host "2. Kubernetes is using an old image (check imagePullPolicy)" -ForegroundColor White
    Write-Host "3. The CORS configuration is not being loaded from environment variables" -ForegroundColor White
    Write-Host ""
    Write-Host "Required actions:" -ForegroundColor Yellow
    Write-Host "1. Rebuild: docker build -t courtrental-server:latest -f Dockerfile ." -ForegroundColor Cyan
    Write-Host "2. Tag: docker tag courtrental-server:latest $Image" -ForegroundColor Cyan
    Write-Host "3. Push: docker push $Image" -ForegroundColor Cyan
    Write-Host "4. Update: kubectl apply -f k8s\server-deployment.yaml -n $Namespace" -ForegroundColor Cyan
    Write-Host "5. Restart: kubectl rollout restart deployment/$Deployment -n $Namespace" -ForegroundColor Cyan
    Write-Host "6. Wait: kubectl rollout status deployment/$Deployment -n $Namespace" -ForegroundColor Cyan
}

Write-Host ""
Write-Host "=========================================" -ForegroundColor Cyan
