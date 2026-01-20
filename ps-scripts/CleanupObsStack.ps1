# Variables
$namespace = "meniga-getting-started-monitoring"

# Function to stop the environment
function Stop-Environment {
    Write-Output "Stopping all observability stack components..."
    
    # Uninstall Helm releases
    Write-Output "Uninstalling Helm releases..."
    & helm uninstall promtail --namespace $namespace
    & helm uninstall tempo --namespace $namespace
    & helm uninstall loki --namespace $namespace
    & helm uninstall kube-prometheus-stack --namespace $namespace

    # Delete namespace
    Write-Output "Deleting namespace $namespace..."
    & kubectl delete namespace $namespace

    Write-Output "All components have been stopped and namespace $namespace deleted, remember to remove unnecessary images if needed."
}

# Main script execution
Stop-Environment

Write-Output "Environment stopped successfully!"
