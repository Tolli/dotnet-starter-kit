# Sample observability stack
> [!IMPORTANT]
> The instructions in this section are intended for sandbox/dev/test environment only<br/>
> This stack is optional, only served as an example & not managed or maintained by Meniga

This operational stack consists of:
- Promtail - collects logs & metrics from cluster resources
- Loki - Log aggregation for analysis and monitoring
- kube-prometheus-stack - containing of:
  - [kube-state-metrics](https://github.com/prometheus-community/helm-charts/tree/main/charts/kube-state-metrics) - service which listens to the Kubernetes API server and generates metrics
  - [Prometheus node exporter](https://github.com/prometheus-community/helm-charts/tree/main/charts/prometheus-node-exporter) - exporter for hardware and os metrics exposed by *NIX kernels
  - [Prometheus](https://prometheus.io/) - Systems monitoring and alerting toolkit
  - [Grafana](https://grafana.com/) - Powerful dashboard visualization tool for querying, visualizing, alerting on, and exploring your 
metrics, logs and traces

## Create namespace

Create namespace if it does not already exist, e.g. `meniga-getting-started-monitoring`

If using custom namespace for solution and/or observability stack you need to adjust hostnames accordingly in
- [`../infrastructure/observability-stack/kube-prometheus-stack/values.yaml`](../infrastructure/observability-stack/kube-prometheus-stack/values.yaml)
- [`../infrastructure/observability-stack/promtail/values.yaml`](../infrastructure/observability-stack/promtail/values.yaml)
- [`../infrastructure/observability-stack/sql-exporter/values.yaml`](../infrastructure/observability-stack/sql-exporter/values.yaml)
- [`../meniga/meniga-hub/meniga-hub-values-obs.yaml`](../meniga/meniga-hub/meniga-hub-values-obs.yaml)
- [`../meniga/insights-factory/insights-factory-values-obs.yaml`](../meniga/insights-factory/insights-factory-values-obs.yaml)

```pwsh
kubectl create namespace meniga-getting-started-monitoring
```
 
## Install Promtail
- Instructions taken from [Install Promtail](https://grafana.com/docs/loki/latest/send-data/promtail/installation/)

- Add Grafana’s helm repository

```pwsh
helm repo add grafana https://grafana.github.io/helm-charts
```

- Update the chart repository

```pwsh
helm repo update
```

- Validate url value for Loki in [`./promtail/values.yaml`](./promtail/values.yaml)

- Deploy Promtail

```pwsh
helm upgrade --install promtail grafana/promtail --values ./infrastructure/observability-stack/promtail/values.yaml -n meniga-getting-started-monitoring
```

To see all pods created by the deployment execute the following statement and check they are all running

```pwsh
kubectl get pods -l app.kubernetes.io/instance=promtail -n meniga-getting-started-monitoring
```

Example output:

```
NAME             READY   STATUS    RESTARTS   AGE
promtail-gfpv7   1/1     Running   0          130m
```

## Install Tempo

- Deploy Tempo

```pwsh
helm upgrade --install tempo grafana/tempo --values $baseDir/infrastructure/observability-stack/tempo/values.yaml -n meniga-getting-started-monitoring
```

To see all pods created by the deployment execute the following statement and check they are all running

```pwsh
kubectl get pods -l app.kubernetes.io/instance=tempo -n meniga-getting-started-monitoring
```

Example output:

```
NAME             READY   STATUS    RESTARTS   AGE
tempo-dcvg3      1/1     Running   0          130m
```

## Install Loki
- Instructions taken from [Install the monolithic Helm chart](https://grafana.com/docs/loki/latest/setup/install/helm/install-monolithic/)

- Deploy Loki

```pwsh
helm upgrade --install loki grafana/loki --values ./infrastructure/observability-stack/loki/values.yaml -n meniga-getting-started-monitoring
```

To see all pods created by the deployment execute the following statement and check they are all running

```pwsh
kubectl get pods -l app.kubernetes.io/instance=loki -n meniga-getting-started-monitoring
```

Example output:

```
NAME                            READY   STATUS    RESTARTS   AGE
loki-0                          2/2     Running   0          136m
loki-canary-zsjvs               1/1     Running   0          136m
loki-chunks-cache-0             2/2     Running   0          136m
loki-gateway-5df7686d97-gqfcd   1/1     Running   0          136m
loki-results-cache-0            2/2     Running   0          136m
```

## Install kube-prometheus-stack
- Instructions taken from [kube-prometheus-stack](https://github.com/prometheus-community/helm-charts/blob/main/charts/kube-prometheus-stack/README.md#configuration)

- Add kube-prometheus-stack helm repository

```pwsh
helm repo add prometheus-community https://prometheus-community.github.io/helm-charts
```

- Update the chart repository

```pwsh
helm repo update
```

- Deploy kube-prometheus-stack

```pwsh
helm upgrade --install kube-prometheus-stack prometheus-community/kube-prometheus-stack --values ./infrastructure/observability-stack/kube-prometheus-stack/values.yaml -n meniga-getting-started-monitoring
```

To see the pods created by the deployment execute the following statement and check they are all running

```pwsh
kubectl get pods -l app.kubernetes.io/instance=kube-prometheus-stack -n meniga-getting-started-monitoring
```

Example output:

```
NAME                                                        READY   STATUS    RESTARTS   AGE
kube-prometheus-stack-grafana-64d8d646d-xrg5k               3/3     Running   0          130m
kube-prometheus-stack-kube-state-metrics-76bf68bd74-s797w   1/1     Running   0          130m
kube-prometheus-stack-operator-c9484b76b-llwfc              1/1     Running   0          130m
```

## Grafana dashboards

Load grafana dashboards

```
kubectl kustomize --load-restrictor=LoadRestrictionsNone ./infrastructure/observability-stack/dashboards | kubectl apply -f $_ - -n meniga-getting-started-monitoring
```

## Test observability stack

To see the ingresses created by the deployment execute the following statement:

```pwsh
kubectl get ing -l app.kubernetes.io/instance=kube-prometheus-stack -n meniga-getting-started-monitoring
```

Example output:

```
NAME                                 CLASS   HOSTS                             ADDRESS     PORTS   AGE
kube-prometheus-stack-alertmanager   nginx   alertmanager.k8s.tbr.localhost     localhost   80      130m
kube-prometheus-stack-grafana        nginx   grafana.k8s.tbr.localhost          localhost   80      130m
kube-prometheus-stack-prometheus     nginx   prometheus.k8s.tbr.localhost       localhost   80      130m
```

You may need to edit entries in `hosts` file on your machine or in DNS to use the ip addresses of the ingresses. See [`../../hostnames/README.md`](../../hostnames/README.md).

Test connecting to Grafana using browser at [http://grafana.k8s.tbr.localhost/](http://grafana.k8s.tbr.localhost/)
- Username: `admin`
- Password: `rootAdmin321`

## Enable observability stack in modules

You need to add monitoring related configuration and apply the helm chart for modules

### Meniga Hub

```pwsh
helm upgrade --install meniga-hub oci://registry-1.docker.io/menigaehf/meniga-hub --version 0.6.1 --values ./meniga/meniga-hub/meniga-hub-values-obs.yaml
```

### Insights Factory

```pwsh
helm upgrade --install insights-factory oci://registry-1.docker.io/menigaehf/insights-factory --version 0.6.3 --values ./meniga/insights-factory/insights-factory-values-obs.yaml
```

### Cashflow Assistant

```pwsh
helm upgrade --install cashflow-assistant oci://registry-1.docker.io/menigaehf/cashflow-assistant --values ./meniga/cashflow-assistant/cashflow-assistant-values-obs.yaml
```
