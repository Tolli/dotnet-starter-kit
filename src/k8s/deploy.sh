#!/bin/bash

# Build and deploy FSH WebAPI to Kubernetes

set -e

# Configuration
NAMESPACE="${NAMESPACE:-fsh-webapi}"
IMAGE_NAME="${IMAGE_NAME:-fsh-webapi}"
IMAGE_TAG="${IMAGE_TAG:-latest}"
REGISTRY="${REGISTRY:-}"

echo "?? Building and deploying FSH WebAPI to Kubernetes"
echo "Namespace: $NAMESPACE"
echo "Image: ${REGISTRY:+$REGISTRY/}$IMAGE_NAME:$IMAGE_TAG"

# Step 1: Build Docker image
echo ""
echo "?? Building Docker image..."
docker build -t ${REGISTRY:+$REGISTRY/}$IMAGE_NAME:$IMAGE_TAG -f Dockerfile .

# Step 2: Push to registry (skip if no registry specified or using minikube)
if [ -n "$REGISTRY" ]; then
    echo ""
    echo "??  Pushing image to registry..."
    docker push ${REGISTRY:+$REGISTRY/}$IMAGE_NAME:$IMAGE_TAG
elif command -v minikube &> /dev/null && minikube status &> /dev/null; then
    echo ""
    echo "?? Loading image into minikube..."
    minikube image load ${IMAGE_NAME}:${IMAGE_TAG}
fi

# Step 3: Create namespace if it doesn't exist
echo ""
echo "?? Creating namespace..."
kubectl create namespace $NAMESPACE --dry-run=client -o yaml | kubectl apply -f -

# Step 4: Apply Kubernetes manifests
echo ""
echo "??  Deploying to Kubernetes..."

# Deploy in order
echo "  - Applying ConfigMap..."
kubectl apply -f k8s/server-configmap.yaml -n $NAMESPACE

echo "  - Applying Secrets..."
kubectl apply -f k8s/server-secret.yaml -n $NAMESPACE

echo "  - Applying Deployment..."
# Update image in deployment if using custom registry
if [ -n "$REGISTRY" ]; then
    sed "s|image: webapi:latest|image: ${REGISTRY}/${IMAGE_NAME}:${IMAGE_TAG}|g" k8s/server-deployment.yaml | kubectl apply -f - -n $NAMESPACE
else
    sed "s|image: webapi:latest|image: ${IMAGE_NAME}:${IMAGE_TAG}|g" k8s/server-deployment.yaml | kubectl apply -f - -n $NAMESPACE
fi

echo "  - Applying Service..."
kubectl apply -f k8s/server-service.yaml -n $NAMESPACE

# Optional: Deploy ingress if file exists and DEPLOY_INGRESS is set
if [ -f "k8s/server-ingress.yaml" ] && [ "$DEPLOY_INGRESS" = "true" ]; then
    echo "  - Applying Ingress..."
    kubectl apply -f k8s/server-ingress.yaml -n $NAMESPACE
fi

# Optional: Deploy database if DEPLOY_DATABASE is set
if [ "$DEPLOY_DATABASE" = "true" ]; then
    echo "  - Deploying SQL Server..."
    kubectl apply -f k8s/mssql-deployment.yaml -n $NAMESPACE
fi

# Step 5: Wait for deployment to be ready
echo ""
echo "? Waiting for deployment to be ready..."
kubectl rollout status deployment/fsh-webapi -n $NAMESPACE --timeout=300s

# Step 6: Show status
echo ""
echo "? Deployment complete!"
echo ""
echo "?? Status:"
kubectl get pods -n $NAMESPACE -l app=fsh-webapi
echo ""
kubectl get services -n $NAMESPACE -l app=fsh-webapi

# Show access instructions
echo ""
echo "?? Access the application:"
echo ""
echo "Using port-forward:"
echo "  kubectl port-forward service/fsh-webapi 8080:80 -n $NAMESPACE"
echo "  Then access: http://localhost:8080"
echo ""

if [ "$DEPLOY_INGRESS" = "true" ]; then
    INGRESS_HOST=$(kubectl get ingress fsh-webapi-ingress -n $NAMESPACE -o jsonpath='{.spec.rules[0].host}' 2>/dev/null || echo "")
    if [ -n "$INGRESS_HOST" ]; then
        echo "Using Ingress:"
        echo "  https://$INGRESS_HOST"
        echo ""
    fi
fi

echo "?? View logs:"
echo "  kubectl logs -f deployment/fsh-webapi -n $NAMESPACE"
echo ""
