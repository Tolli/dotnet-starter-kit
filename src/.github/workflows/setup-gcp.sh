#!/bin/bash

# Setup script for GitHub Actions GCP Service Account

set -e

# Configuration
PROJECT_ID="main-project-483817"
SERVICE_ACCOUNT_NAME="github-actions"
SERVICE_ACCOUNT_EMAIL="${SERVICE_ACCOUNT_NAME}@${PROJECT_ID}.iam.gserviceaccount.com"
KEY_FILE="github-actions-key.json"

echo "========================================="
echo "GitHub Actions GCP Setup Script"
echo "========================================="
echo "Project ID: $PROJECT_ID"
echo "Service Account: $SERVICE_ACCOUNT_EMAIL"
echo "========================================="
echo ""

# Check if gcloud is installed
if ! command -v gcloud &> /dev/null; then
    echo "? gcloud CLI is not installed. Please install it first:"
    echo "   https://cloud.google.com/sdk/docs/install"
    exit 1
fi

# Set the project
echo "1??  Setting GCP project..."
gcloud config set project $PROJECT_ID
echo "? Project set to $PROJECT_ID"
echo ""

# Check if service account exists
echo "2??  Checking if service account exists..."
if gcloud iam service-accounts describe $SERVICE_ACCOUNT_EMAIL &> /dev/null; then
    echo "??  Service account already exists"
else
    echo "Creating service account..."
    gcloud iam service-accounts create $SERVICE_ACCOUNT_NAME \
        --display-name="GitHub Actions Service Account" \
        --description="Service account for GitHub Actions CI/CD"
    echo "? Service account created"
fi
echo ""

# Grant Artifact Registry Writer role
echo "3??  Granting Artifact Registry Writer role..."
gcloud projects add-iam-policy-binding $PROJECT_ID \
    --member="serviceAccount:$SERVICE_ACCOUNT_EMAIL" \
    --role="roles/artifactregistry.writer" \
    --condition=None \
    > /dev/null
echo "? Artifact Registry Writer role granted"
echo ""

# Grant Container Developer role (for GKE deployments)
echo "4??  Granting Kubernetes Engine Developer role..."
gcloud projects add-iam-policy-binding $PROJECT_ID \
    --member="serviceAccount:$SERVICE_ACCOUNT_EMAIL" \
    --role="roles/container.developer" \
    --condition=None \
    > /dev/null
echo "? Kubernetes Engine Developer role granted"
echo ""

# Grant Storage Object Viewer role (for GKE)
echo "5??  Granting Storage Object Viewer role..."
gcloud projects add-iam-policy-binding $PROJECT_ID \
    --member="serviceAccount:$SERVICE_ACCOUNT_EMAIL" \
    --role="roles/storage.objectViewer" \
    --condition=None \
    > /dev/null
echo "? Storage Object Viewer role granted"
echo ""

# Create and download key
echo "6??  Creating service account key..."
if [ -f "$KEY_FILE" ]; then
    echo "??  Key file already exists. Backing up..."
    mv $KEY_FILE "${KEY_FILE}.backup.$(date +%s)"
fi

gcloud iam service-accounts keys create $KEY_FILE \
    --iam-account=$SERVICE_ACCOUNT_EMAIL

echo "? Service account key created: $KEY_FILE"
echo ""

# Display the key content
echo "========================================="
echo "?? Service Account Key"
echo "========================================="
echo ""
echo "Copy the contents below and add it as a GitHub Secret named 'GCP_SA_KEY':"
echo ""
cat $KEY_FILE
echo ""
echo "========================================="
echo ""

# Instructions
echo "?? Next Steps:"
echo ""
echo "1. Go to your GitHub repository"
echo "2. Navigate to Settings ? Secrets and variables ? Actions"
echo "3. Click 'New repository secret'"
echo "4. Name: GCP_SA_KEY"
echo "5. Value: Paste the JSON content from above"
echo "6. Click 'Add secret'"
echo ""
echo "7. Update the workflow files (.github/workflows/deploy-*.yml):"
echo "   - Set GKE_CLUSTER to your cluster name"
echo "   - Set GKE_ZONE to your cluster zone"
echo "   - Set NAMESPACE if not using 'default'"
echo ""
echo "?? Setup complete! You can now use GitHub Actions to deploy."
echo ""

# Cleanup option
read -p "Delete the key file from local machine? (y/N): " -n 1 -r
echo
if [[ $REPLY =~ ^[Yy]$ ]]; then
    rm $KEY_FILE
    echo "? Key file deleted. Make sure you saved it to GitHub Secrets!"
else
    echo "??  Remember to delete $KEY_FILE after adding it to GitHub Secrets!"
fi
