$namespace = "meniga-getting-started-monitoring"

# Be able to run the script from root or ps-scripts folder
$baseDir = (Get-Location).Path
if ($baseDir -like "*\ps-scripts") {
    $baseDir = $baseDir -replace "\\ps-scripts", ""
}

kubectl delete configmap -n $namespace -l custom_dashboard="1"

kubectl kustomize --load-restrictor=LoadRestrictionsNone $baseDir/infrastructure/observability-stack/dashboards | kubectl apply -f $_ - -n $namespace
