#!/bin/bash

# Comprehensive CORS diagnostic script for Kubernetes deployment

NAMESPACE="${1:-default}"
DEPLOYMENT="${2:-fsh-webapi}"
API_URL="${3:-https://tbr-api.tolli.com}"
ORIGIN="${4:-https://tbr.tolli.com}"

echo "========================================="
echo "?? CORS Diagnostic Tool"
echo "========================================="
echo "Namespace: $NAMESPACE"
echo "Deployment: $DEPLOYMENT"
echo "API URL: $API_URL"
echo "Origin: $ORIGIN"
echo "========================================="
echo ""

# Step 1: Check if deployment exists
echo "1??  Checking if deployment exists..."
if ! kubectl get deployment $DEPLOYMENT -n $NAMESPACE &> /dev/null; then
    echo "? Deployment $DEPLOYMENT not found in namespace $NAMESPACE"
    exit 1
fi
echo "? Deployment found"
echo ""

# Step 2: Check pod status
echo "2??  Checking pod status..."
kubectl get pods -l app=$DEPLOYMENT -n $NAMESPACE
echo ""

# Step 3: Get pod name
POD=$(kubectl get pods -l app=$DEPLOYMENT -n $NAMESPACE -o jsonpath='{.items[0].metadata.name}' 2>/dev/null)
if [ -z "$POD" ]; then
    echo "? No running pods found"
    exit 1
fi
echo "Using pod: $POD"
echo ""

# Step 4: Check image version
echo "3??  Checking Docker image..."
IMAGE=$(kubectl get pod $POD -n $NAMESPACE -o jsonpath='{.spec.containers[0].image}')
echo "Current image: $IMAGE"
echo ""

# Step 5: Check when pod was created
echo "4??  Checking pod age..."
kubectl get pod $POD -n $NAMESPACE -o jsonpath='{.metadata.creationTimestamp}'
echo ""
echo ""

# Step 6: Check CORS environment variables
echo "5??  Checking CORS environment variables in pod..."
kubectl exec $POD -n $NAMESPACE -- env | grep -i cors || echo "No CORS environment variables found"
echo ""

# Step 7: Check all relevant environment variables
echo "6??  Checking all CorsOptions environment variables..."
kubectl exec $POD -n $NAMESPACE -- env | grep "CorsOptions" || echo "No CorsOptions environment variables found"
echo ""

# Step 8: Check pod logs for CORS-related messages
echo "7??  Checking pod logs for CORS messages..."
kubectl logs $POD -n $NAMESPACE --tail=100 | grep -i cors || echo "No CORS messages in logs"
echo ""

# Step 9: Test actual CORS with curl
echo "8??  Testing CORS with preflight request..."
echo "Sending OPTIONS request to: $API_URL/api/token"
echo "From origin: $ORIGIN"
echo ""

RESPONSE=$(curl -v -X OPTIONS "$API_URL/api/token" \
  -H "Origin: $ORIGIN" \
  -H "Access-Control-Request-Method: POST" \
  -H "Access-Control-Request-Headers: tenant,content-type,authorization" \
  2>&1)

echo "$RESPONSE"
echo ""

# Step 10: Parse and check CORS headers
echo "9??  Analyzing CORS response headers..."
echo ""

if echo "$RESPONSE" | grep -qi "Access-Control-Allow-Origin:.*$ORIGIN"; then
  echo "? Access-Control-Allow-Origin: $ORIGIN is present"
else
  echo "? Access-Control-Allow-Origin: $ORIGIN is MISSING"
  echo "   Found: $(echo "$RESPONSE" | grep -i "Access-Control-Allow-Origin:" || echo "Not found")"
fi

if echo "$RESPONSE" | grep -qi "Access-Control-Allow-Headers:.*tenant"; then
  echo "? Access-Control-Allow-Headers includes 'tenant'"
else
  echo "? Access-Control-Allow-Headers does NOT include 'tenant'"
  echo "   This is the ROOT CAUSE of your problem!"
  echo "   Found: $(echo "$RESPONSE" | grep -i "Access-Control-Allow-Headers:" || echo "Not found")"
fi

if echo "$RESPONSE" | grep -qi "Access-Control-Allow-Credentials:.*true"; then
  echo "? Access-Control-Allow-Credentials: true is present"
else
  echo "? Access-Control-Allow-Credentials is MISSING or false"
  echo "   Found: $(echo "$RESPONSE" | grep -i "Access-Control-Allow-Credentials:" || echo "Not found")"
fi

if echo "$RESPONSE" | grep -qi "Access-Control-Allow-Methods:.*POST"; then
  echo "? Access-Control-Allow-Methods includes 'POST'"
else
  echo "? Access-Control-Allow-Methods does NOT include 'POST'"
  echo "   Found: $(echo "$RESPONSE" | grep -i "Access-Control-Allow-Methods:" || echo "Not found")"
fi

echo ""
echo "?? All CORS-related response headers:"
echo "$RESPONSE" | grep -i "access-control" | sed 's/^[<>]* //'
echo ""

# Step 11: Summary
echo "========================================="
echo "?? Summary"
echo "========================================="
echo ""

if echo "$RESPONSE" | grep -qi "Access-Control-Allow-Headers:.*tenant"; then
    echo "? CORS is configured correctly!"
    echo "   If you're still seeing errors in the browser:"
    echo "   1. Clear browser cache"
    echo "   2. Try incognito/private mode"
    echo "   3. Check browser DevTools console for other errors"
else
    echo "? CORS is NOT configured correctly!"
    echo ""
    echo "The API is not allowing the 'tenant' header."
    echo ""
    echo "Possible causes:"
    echo "1. The Docker image hasn't been rebuilt with the new CORS code"
    echo "2. Kubernetes is using an old image (check imagePullPolicy)"
    echo "3. The CORS configuration is not being loaded from environment variables"
    echo ""
    echo "Required actions:"
    echo "1. Rebuild: docker build -t courtrental-server:latest -f Dockerfile ."
    echo "2. Tag: docker tag courtrental-server:latest $IMAGE"
    echo "3. Push: docker push $IMAGE"
    echo "4. Update: kubectl apply -f k8s/server-deployment.yaml -n $NAMESPACE"
    echo "5. Restart: kubectl rollout restart deployment/$DEPLOYMENT -n $NAMESPACE"
    echo "6. Wait: kubectl rollout status deployment/$DEPLOYMENT -n $NAMESPACE"
fi

echo ""
echo "========================================="
