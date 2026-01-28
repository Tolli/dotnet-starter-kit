# FSH WebAPI - Kubernetes Deployment Guide

This directory contains Kubernetes manifests for deploying the FSH Starter WebAPI to a Kubernetes cluster.

## Prerequisites

- A running Kubernetes cluster (minikube, AKS, EKS, GKE, etc.)
- `kubectl` configured to access your cluster
- Docker image built and pushed to a container registry (or available locally for minikube)

## Files Overview

- `server-deployment.yaml` - Main application deployment
- `server-service.yaml` - ClusterIP service for the API
- `server-configmap.yaml` - Non-sensitive configuration
- `server-secret.yaml` - Sensitive configuration (passwords, keys)
- `server-ingress.yaml` - Ingress for external access (optional)
- `mssql-deployment.yaml` - SQL Server deployment (for development/testing)

## Quick Start

### 1. Build and Push Docker Image

```bash
# Build the image
docker build -t your-registry/fsh-webapi:latest -f Dockerfile .

# Push to registry (skip for minikube)
docker push your-registry/fsh-webapi:latest

# For minikube, load image directly
minikube image load your-registry/fsh-webapi:latest
```

### 2. Update Configuration

Edit the following files before deploying:

**server-secret.yaml** - Update all secrets:
```bash
# Generate a new JWT key
openssl rand -base64 32

# Update database connection string
# Update mail credentials
# Update Hangfire credentials
```

**server-configmap.yaml** - Update configuration:
- Database provider (mssql/postgresql)
- Mail server settings
- Redis connection (if using)
- Allowed CORS origins

**server-deployment.yaml** - Update image reference:
```yaml
image: your-registry/fsh-webapi:latest
```

**server-ingress.yaml** - Update domain:
```yaml
host: api.yourdomain.com
```

### 3. Deploy to Kubernetes

```bash
# Create namespace (optional)
kubectl create namespace fsh-webapi

# Deploy database (optional - for dev/test only)
kubectl apply -f k8s/mssql-deployment.yaml -n fsh-webapi

# Deploy application
kubectl apply -f k8s/server-configmap.yaml -n fsh-webapi
kubectl apply -f k8s/server-secret.yaml -n fsh-webapi
kubectl apply -f k8s/server-deployment.yaml -n fsh-webapi
kubectl apply -f k8s/server-service.yaml -n fsh-webapi

# Deploy ingress (if using)
kubectl apply -f k8s/server-ingress.yaml -n fsh-webapi
```

### 4. Verify Deployment

```bash
# Check pods
kubectl get pods -n fsh-webapi

# Check services
kubectl get services -n fsh-webapi

# Check logs
kubectl logs -f deployment/fsh-webapi -n fsh-webapi

# Check ingress
kubectl get ingress -n fsh-webapi
```

## Access the Application

### Using Port-Forward (Development)

```bash
kubectl port-forward service/fsh-webapi 8080:80 -n fsh-webapi
# Access at http://localhost:8080
```

### Using Ingress (Production)

Access via the configured domain: `https://api.yourdomain.com`

### Using LoadBalancer (Cloud Providers)

Change service type in `server-service.yaml`:
```yaml
spec:
  type: LoadBalancer
```

## Health Checks

The application exposes health check endpoints:
- `/health` - Liveness probe
- `/health/ready` - Readiness probe

## Scaling

Scale the deployment:
```bash
kubectl scale deployment fsh-webapi --replicas=3 -n fsh-webapi
```

## Updates

Update the application:
```bash
# Build and push new image
docker build -t your-registry/fsh-webapi:v2 -f Dockerfile .
docker push your-registry/fsh-webapi:v2

# Update deployment
kubectl set image deployment/fsh-webapi webapi=your-registry/fsh-webapi:v2 -n fsh-webapi

# Or apply updated manifest
kubectl apply -f k8s/server-deployment.yaml -n fsh-webapi
```

## Database Migrations

Run database migrations as a Kubernetes Job:
```bash
# Create migration job (example)
kubectl run migration --image=your-registry/fsh-webapi:latest \
  --restart=Never \
  --env="ASPNETCORE_ENVIRONMENT=Production" \
  --command -- dotnet ef database update
```

## Monitoring

View logs:
```bash
# All pods
kubectl logs -l app=fsh-webapi -n fsh-webapi --tail=100 -f

# Specific pod
kubectl logs <pod-name> -n fsh-webapi -f
```

## Secrets Management

For production, consider using:
- **Azure Key Vault** with CSI driver
- **AWS Secrets Manager** with External Secrets Operator
- **HashiCorp Vault**
- **Sealed Secrets** for GitOps

## Helm Chart (Optional)

Consider creating a Helm chart for easier deployment management:
```bash
helm create fsh-webapi
# Move manifests to templates/
# Add values.yaml for configuration
```

## Production Considerations

1. **Use managed database** (Azure SQL, RDS, Cloud SQL) instead of in-cluster SQL Server
2. **Enable TLS/HTTPS** with cert-manager
3. **Set resource limits** appropriately
4. **Configure autoscaling** (HPA)
5. **Use secrets management** solution
6. **Enable monitoring** (Prometheus, Application Insights)
7. **Configure backup** strategy
8. **Use multiple replicas** for high availability
9. **Configure network policies**
10. **Enable pod security policies**

## Troubleshooting

### Pod not starting
```bash
kubectl describe pod <pod-name> -n fsh-webapi
kubectl logs <pod-name> -n fsh-webapi
```

### Database connection issues
```bash
# Test database connectivity
kubectl run -it --rm debug --image=mcr.microsoft.com/mssql-tools --restart=Never -- /bin/bash
sqlcmd -S sql-server -U sa -P 'YourPassword'
```

### Service not accessible
```bash
kubectl get endpoints fsh-webapi -n fsh-webapi
kubectl describe service fsh-webapi -n fsh-webapi
```

## Clean Up

```bash
# Delete all resources
kubectl delete -f k8s/ -n fsh-webapi

# Delete namespace
kubectl delete namespace fsh-webapi
```
