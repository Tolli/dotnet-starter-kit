# Deploy Meniga solution
> [!IMPORTANT]
> The instructions in this section are intended for sandbox/dev/test environment only<br/>

This folder contains a few PowerShell scripts to manage Meniga solution deployment:
- [`InstallTools.ps1`](InstallTools.ps1) to install tools
- [`EditHostsFile.ps1`](EditHostsFile.ps1) to add hostnames to `hosts` file
- [`MenigaScripts.ps1`](MenigaScripts.ps1) functions used by [`DeployMenigaDatabases.ps1`](DeployMenigaDatabases.ps1) 
- [`DeployInfrastructure.ps1`](DeployInfrastructure.ps1) to create infrastructure components. Can be run independently when updates are made and is used by [`DeploySolution.ps1`](DeploySolution.ps1)
- [`DeployMenigaDatabases.ps1`](DeployMenigaDatabases.ps1) to create Meniga solution databases and is used by [`DeploySolution.ps1`](DeploySolution.ps1)
- [`DeployMenigaApplications.ps1`](DeployMenigaDatabases.ps1) to create Meniga solution applications. Can be run independently when updates are made and is used by [`DeploySolution.ps1`](DeploySolution.ps1)
- [`DeploySolution.ps1`](DeploySolution.ps1) (re)creates everything needed for Meniga solution (Infrastructure, Databases, Applications)
- [`CleanupSolution.ps1`](CleanupSolution.ps1) cleanup script to delete the Meniga solution namespace
- [`DeployObsStack.ps1`](DeployObsStack.ps1) to deploy observability stack
- [`RefreshDashboards.ps1`](RefreshDashboards.ps1) to refresh observability stack dashboards when they are changed
- [`CleanupObsStack.ps1`](CleanupObsStack.ps1) cleanup script to delete the observability stack

## Install tools

Adjust the script [`InstallTools.ps1`](InstallTools.ps1) according to which tools you want installed.

Install tools by running the script which opens another window requesting elevated privileges

```pwsh
.\ps-scripts\InstallTools.ps1
```

## Edit hosts file

Adjust the script [`EditHostsFile.ps1`](EditHostsFile.ps1) if using `hosts` file and not already performed step in [Configure hostnames](../hostnames/README.md).

Add hostnames to `hosts` file by running the script which opens another window requesting elevated privileges

```pwsh
.\ps-scripts\EditHostsFile.ps1
```

## Configure namespace

If using custom namespace for solution you need to adjust hostnames accordingly in
- [`../meniga/meniga-hub/init-configs/meniga-global.env`](../meniga/meniga-hub/init-configs/meniga-global.env)
- Meniga Hub Helm Values
  - [`../meniga/meniga-hub/meniga-hub-values.yaml`](../meniga/meniga-hub/meniga-hub-values.yaml)
  - Or [`../meniga/meniga-hub/meniga-hub-values-all.yaml`](../meniga/meniga-hub/meniga-hub-values-all.yaml)
  - Or [`../meniga/meniga-hub/meniga-hub-values-obs.yaml`](../meniga/meniga-hub/meniga-hub-values-obs.yaml)
- Insights Factory Helm Values
  - [`../meniga/meniga-hub/insights-factory/insights-factory-values.yaml`](../meniga/meniga-hub/insights-factory/insights-factory-values.yaml)
  - Or [`../meniga/meniga-hub/insights-factory/insights-factory-values-obs.yaml`](../meniga/meniga-hub/insights-factory/insights-factory-values-obs.yaml)
- [`../infrastructure/kafka/helm-values/kafka-ui-values.yaml`](../infrastructure/kafka/helm-values/kafka-ui-values.yaml)

## Deploy solution to local machine

The deploy script [`DeploySolution.ps1`](DeploySolution.ps1) has three optional parameters:
- `deployCA`
  -  default: `$true`
- `deployIF`
  -  default: `$true`
- `deploySMR`
  -  default: `$true`
- `namespace`
  -  default: `meniga-getting-started`

Deploy infrastructure/databases/applications with default parameters values
```pwsh
.\ps-scripts\DeploySolution.ps1
```

## Deploy solution to cloud service provider (Azure/AWS/GCP)

The deploy script [`DeployInfrastructure.ps1`](DeployInfrastructure.ps1) has one optional parameter:
- `namespace`
  -  default: `meniga-getting-started`

Deploy infrastructure with default parameter value
```pwsh
.\ps-scripts\DeployInfrastructure.ps1
```

Get external ip of mssql-0 and change `SQLInstanceName` in `DeployMenigaDatabases.ps1`.

The deploy script [`DeployMenigaDatabases.ps1`](DeployMenigaDatabases.ps1) has two optional parameters:
- `deployCore`
  -  default: `$false`
- `deployCA`
  -  default: `$false`
- `deployIF`
  -  default: `$false`
- `deploySMR`
  -  default: `$false`

Deploy Meniga solution databases with specific parameters values

change ps-scripts\DeployMenigaDatabases $SQLInstanceName to public ip address of sql server or hostname
$SQLInstanceName = "<your server ip or dns alias here>"

Execute DeployMenigaDatabases.ps1
```pwsh
.\ps-scripts\DeployMenigaDatabases.ps1 -deployCore $true -deployCA $true -deployIF $true -deploySMR $true
```

Check IP address (Cluster IP) of DNS

```pwsh
kubectl get svc -n kube-system kube-dns -o jsonpath='{.spec.clusterIP}'
```

Set IP address of DNS as value of `dnsResolver` in
- [`../meniga/meniga-hub/meniga-hub-values.yaml`](../meniga/meniga-hub/meniga-hub-values.yaml)
  - Or [`../meniga/meniga-hub/meniga-hub-values-all.yaml`](../meniga/meniga-hub/meniga-hub-values-all.yaml)
  - Or [`../meniga/meniga-hub/meniga-hub-values-obs.yaml`](../meniga/meniga-hub/meniga-hub-values-obs.yaml)

The deploy script [`DeployMenigaApplications.ps1`](DeployMenigaApplications.ps1) has three optional parameters:
- `deployCA`
  -  default: `$true`
- `deployIF`
  -  default: `$true`
- `deploySMR`
  -  default: `$true`
- `namespace`
  - default: `meniga-getting-started`
 
Deploy Meniga solution applications with default parameters values

```pwsh
.\ps-scripts\DeployMenigaApplications.ps1 -deployIF true -deployCA true -deploySMR true
```

## Hostnames

You may need to edit entries in `hosts` file on your machine or in DNS to use the ip addresses of the ingresses. See [`../hostnames/README.md`](../hostnames/README.md).

## Update infrastructure configuration

Applying updates to infrastructure configuration

```pwsh
.\ps-scripts\DeployInfrastructure.ps1
```

## Update Meniga solution applications configuration

Applying updates to Meniga solution applications configuration

```pwsh
.\ps-scripts\DeployMenigaApplications.ps1
```

## Deploy observability stack

The observability stack deploy script [`DeployObsStack.ps1`](DeployObsStack.ps1) has one optional parameters:
- `namespace` (default `meniga-getting-started-monitoring`)

Deploy sample observability stack including Loki, Grafana, Prometheus and more with default parameters values

```pwsh
.\ps-scripts\DeployObsStack.ps1
```

Deploy sample observability stack including Loki, Grafana, Prometheus and more with `namespace` parameter value

If using custom namespace for solution and/or observability stack you need to adjust hostnames accordingly in
- [`../infrastructure/observability-stack/kube-prometheus-stack/values.yaml`](../infrastructure/observability-stack/kube-prometheus-stack/values.yaml)
- [`../infrastructure/observability-stack/promtail/values.yaml`](../infrastructure/observability-stack/promtail/values.yaml)
- [`../infrastructure/observability-stack/sql-exporter/values.yaml`](../infrastructure/observability-stack/sql-exporter/values.yaml)
- [`../meniga/meniga-hub/meniga-hub-values-obs.yaml`](../meniga/meniga-hub/meniga-hub-values-obs.yaml)
- [`../meniga/insights-factory/insights-factory-values-obs.yaml`](../meniga/insights-factory/insights-factory-values-obs.yaml)

```pwsh
.\ps-scripts\DeployObsStack.ps1 -namespace my-custom-meniga-monitoring-namespace
```

## Deploy sample Merchant Directory

The script to load sample Merchant Directory is `ImportMerchantDirectory.ps1`

Cmd to execute is:

```pwsh
.\ps-scripts\ImportMerchantDirectory.ps1
```

## Cleanup

The cleanup script [`CleanupSolution.ps1`](CleanupSolution.ps1) deletes the solution namespace

Delete solution namespace

```pwsh
.\ps-scripts\CleanupSolution.ps1
```

The cleanup script [`CleanupObsSolution.ps1`](CleanupObsSolution.ps1) deletes the observability stack namespace

Delete observability stack namespace

```pwsh
.\ps-scripts\CleanupObsSolution.ps1
```
