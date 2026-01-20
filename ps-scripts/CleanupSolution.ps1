# Variables
$namespace = "meniga-getting-started"

# Function to stop the environment
function Stop-Environment {
    Write-Output "Stopping all solution components..."
    
    # Uninstall Helm releases
    Write-Output "Uninstalling Helm releases..."
    & helm uninstall meniga-hub --namespace $namespace
    & helm uninstall kafka-ui --namespace $namespace
    & helm uninstall kafka --namespace $namespace
    & helm uninstall cashflow-assistant --namespace $namespace
    & helm uninstall insights-factory --namespace $namespace
    & helm uninstall smart-money-rules --namespace $namespace
        
    # Delete namespace
    Write-Output "Deleting namespace $namespace..."
    & kubectl delete namespace $namespace

    Write-Output "All components have been stopped and namespace $namespace deleted, remember to remove unnecessary images if needed."
}

# Main script execution
Stop-Environment

Write-Output "Environment stopped successfully!"
