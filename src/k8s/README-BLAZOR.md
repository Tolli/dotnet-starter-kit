# FSH Blazor Client - Kubernetes Deployment Guide

This guide covers deploying the Blazor WebAssembly client to Kubernetes.

## Prerequisites

- Docker installed
- Kubernetes cluster (minikube, AKS, EKS, GKE, etc.)
- `kubectl` configured
- nginx Ingress controller (for external access)

## Files Overview

- `Dockerfile.Blazor` - Multi-stage Dockerfile for Blazor WASM
- `apps/blazor/nginx.conf` - nginx configuration for serving Blazor WASM
- `k8s/blazor-deployment.yaml` - Kubernetes deployment
- `k8s/blazor-service.yaml` - ClusterIP service
- `k8s/blazor-configmap.yaml` - Configuration (API URL, environment)
- `k8s/blazor-ingress.yaml` - Ingress for external access
- `k8s/deploy-blazor.sh` - Automated deployment script

## Quick Start

### 1. Build the Docker Image

```bash
# From solution root
docker build -t fsh-blazor-client:latest -f Dockerfile.Blazor .

# For minikube
minikube image load fsh-blazor-client:latest

# For remote registry
docker tag fsh-blazor-client:latest your-registry/fsh-blazor-client:latest
docker push your-registry/fsh-blazor-client:latest
```

### 2. Configure the Application

Edit `k8s/blazor-configmap.yaml`:
```yaml
data:
  api-base-url: "https://api.yourdomain.com"  # Your API URL
  environment: "Production"
```

Edit `k8s/blazor-ingress.yaml`:
```yaml
spec:
  tls:
  - hosts:
    - app.yourdomain.com  # Your domain
  rules:
  - host: app.yourdomain.com  # Your domain
```

### 3. Deploy to Kubernetes

**Using the deploy script:**
```bash
chmod +x k8s/deploy-blazor.sh
./k8s/deploy-blazor.sh

# With custom settings
NAMESPACE=production IMAGE_TAG=v1.0.0 DEPLOY_INGRESS=true ./k8s/deploy-blazor.sh
```

**Or deploy manually:**
```bash
# Create namespace
kubectl create namespace fsh-blazor

# Deploy
kubectl apply -f k8s/blazor-configmap.yaml -n fsh-blazor
kubectl apply -f k8s/blazor-deployment.yaml -n fsh-blazor
kubectl apply -f k8s/blazor-service.yaml -n fsh-blazor
kubectl apply -f k8s/blazor-ingress.yaml -n fsh-blazor
```

### 4. Verify Deployment

```bash
# Check pods
kubectl get pods -n fsh-blazor

# Check service
kubectl get svc -n fsh-blazor

# Check ingress
kubectl get ingress -n fsh-blazor

# View logs
kubectl logs -f deployment/fsh-blazor-client -n fsh-blazor
```

## Access the Application

### Local Development (Port Forward)

```bash
kubectl port-forward service/fsh-blazor-client 8080:80 -n fsh-blazor
# Access: http://localhost:8080
```

### Production (Ingress)

Access via your configured domain: `https://app.yourdomain.com`

## Configuration

### API Base URL

The Blazor app needs to know where the API is. Configure in `k8s/blazor-configmap.yaml`:

```yaml
data:
  api-base-url: "https://api.yourdomain.com"
```

### Environment Variables

Add environment variables in the deployment:

```yaml
env:
- name: API_BASE_URL
  valueFrom:
    configMapKeyRef:
      name: fsh-blazor-config
      key: api-base-url
```

## Nginx Configuration

The `apps/blazor/nginx.conf` provides:
- **Compression**: gzip for all text/wasm files
- **Caching**: 1 year for static assets, no cache for index.html
- **Security headers**: X-Frame-Options, X-Content-Type-Options, etc.
- **SPA routing**: Proper handling of Blazor routing
- **Health check**: `/health` endpoint for liveness/readiness probes
- **WASM support**: Proper MIME types for WebAssembly files

## Scaling

Scale the deployment:
```bash
kubectl scale deployment fsh-blazor-client --replicas=5 -n fsh-blazor
```

Enable autoscaling:
```bash
kubectl autoscale deployment fsh-blazor-client \
  --cpu-percent=80 \
  --min=2 \
  --max=10 \
  -n fsh-blazor
```

## Updates

Update the application:
```bash
# Build new image
docker build -t fsh-blazor-client:v2 -f Dockerfile.Blazor .
docker push your-registry/fsh-blazor-client:v2

# Update deployment
kubectl set image deployment/fsh-blazor-client \
  blazor-client=your-registry/fsh-blazor-client:v2 \
  -n fsh-blazor

# Or apply updated manifest
kubectl apply -f k8s/blazor-deployment.yaml -n fsh-blazor
```

## TLS/HTTPS

### Using cert-manager (recommended)

1. Install cert-manager:
```bash
kubectl apply -f https://github.com/cert-manager/cert-manager/releases/download/v1.13.0/cert-manager.yaml
```

2. Create ClusterIssuer:
```yaml
apiVersion: cert-manager.io/v1
kind: ClusterIssuer
metadata:
  name: letsencrypt-prod
spec:
  acme:
    server: https://acme-v02.api.letsencrypt.org/directory
    email: your-email@example.com
    privateKeySecretRef:
      name: letsencrypt-prod
    solvers:
    - http01:
        ingress:
          class: nginx
```

3. Update ingress annotation in `k8s/blazor-ingress.yaml`:
```yaml
annotations:
  cert-manager.io/cluster-issuer: "letsencrypt-prod"
```

## Monitoring

### Health Checks

The deployment includes:
- **Liveness probe**: Checks `/health` every 10s
- **Readiness probe**: Checks `/health` every 5s

### Logging

View logs:
```bash
# All pods
kubectl logs -l app=fsh-blazor-client -n fsh-blazor --tail=100 -f

# Specific pod
kubectl logs <pod-name> -n fsh-blazor -f
```

## Troubleshooting

### Pod not starting
```bash
kubectl describe pod <pod-name> -n fsh-blazor
kubectl logs <pod-name> -n fsh-blazor
```

### 404 errors on navigation
Check nginx.conf has proper SPA routing:
```nginx
location / {
    try_files $uri $uri/ /index.html =404;
}
```

### API calls failing
1. Check API URL in ConfigMap
2. Verify CORS settings on API
3. Check network policies

### WASM files not loading
Ensure nginx.conf includes:
```nginx
types {
    application/wasm wasm;
}
```

## Production Considerations

1. **Use CDN**: For better performance globally
2. **Enable caching**: Properly configured in nginx.conf
3. **Compress assets**: gzip enabled in nginx.conf
4. **Security headers**: Already configured
5. **Resource limits**: Set in deployment.yaml
6. **Multiple replicas**: For high availability
7. **Health checks**: Configured for reliability
8. **Monitoring**: Add Prometheus/Grafana
9. **SSL/TLS**: Use cert-manager with Let's Encrypt
10. **Custom domain**: Configure in ingress

## Clean Up

```bash
# Delete all resources
kubectl delete -f k8s/blazor-*.yaml -n fsh-blazor

# Delete namespace
kubectl delete namespace fsh-blazor
```

## Architecture

```
???????????????
?   Users     ?
???????????????
       ?
       ?
???????????????
?   Ingress   ? (nginx-ingress)
???????????????
       ?
       ?
???????????????
?   Service   ? (ClusterIP)
???????????????
       ?
       ?
???????????????
?    Pods     ? (nginx + Blazor WASM)
???????????????
```

Blazor WASM is compiled to static files and served by nginx. All application logic runs in the browser.
