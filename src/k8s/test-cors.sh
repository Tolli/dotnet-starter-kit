#!/bin/bash

# Script to test CORS configuration

API_URL="${1:-https://tbr-api.tolli.com}"
ORIGIN="${2:-https://tbr.tolli.com}"

echo "?? Testing CORS Configuration"
echo "API URL: $API_URL"
echo "Origin: $ORIGIN"
echo ""

echo "?? Sending OPTIONS preflight request..."
echo ""

# Send preflight request
RESPONSE=$(curl -i -X OPTIONS "$API_URL/api/token" \
  -H "Origin: $ORIGIN" \
  -H "Access-Control-Request-Method: POST" \
  -H "Access-Control-Request-Headers: tenant,content-type,authorization" \
  -s)

echo "$RESPONSE"
echo ""

# Check for specific headers
echo "?? Checking CORS headers..."
echo ""

if echo "$RESPONSE" | grep -q "Access-Control-Allow-Origin: $ORIGIN"; then
  echo "? Access-Control-Allow-Origin: $ORIGIN"
else
  echo "? Access-Control-Allow-Origin header missing or wrong value"
fi

if echo "$RESPONSE" | grep -qi "Access-Control-Allow-Headers:.*tenant"; then
  echo "? Access-Control-Allow-Headers includes 'tenant'"
else
  echo "? Access-Control-Allow-Headers does NOT include 'tenant'"
  echo "   This is the problem!"
fi

if echo "$RESPONSE" | grep -q "Access-Control-Allow-Credentials: true"; then
  echo "? Access-Control-Allow-Credentials: true"
else
  echo "? Access-Control-Allow-Credentials header missing"
fi

if echo "$RESPONSE" | grep -qi "Access-Control-Allow-Methods:.*POST"; then
  echo "? Access-Control-Allow-Methods includes 'POST'"
else
  echo "? Access-Control-Allow-Methods does NOT include 'POST'"
fi

echo ""
echo "?? Full Response Headers:"
echo "$RESPONSE" | grep -i "access-control"
