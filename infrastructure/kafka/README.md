# Install Kafka
> [!IMPORTANT]
> The instructions in this section are intended for sandbox/dev/test environment only<br/>
> Use `oc` command instead of `kubectl` command below when using OpenShift<br/>
> If using external Kafka refer instead to [External Kafka](./external.md)<br/>

When deploying addtional modules Kafka or Rabbit MQ are required. The following assumes Kafka is used.

- Add bitnami's helm repository

```pwsh
helm repo add bitnami https://charts.bitnami.com/bitnami
```

- Add Kafka UI helm repository

```pwsh
helm repo add kafka-ui https://provectus.github.io/kafka-ui-charts
```

- Validate environment variables in [`./init-configs`](./init-configs)

- Validate helm values in [`./helm-values/kafka-values.yaml`](./helm-values/kafka-values.yaml) and [`./helm-values/kafka-values-ui.yaml`](./helm-values/kafka-values-ui.yaml)

- Build and apply the YAML for Kafka

```pwsh
kubectl kustomize --load-restrictor=LoadRestrictionsNone ./infrastructure/kafka/init-configs | kubectl apply -f $_ -
```

- If using OpenShift install Kafka with

```pwsh
helm upgrade --install kafka bitnami/kafka --version 25.3.0 `
--values ./infrastructure/kafka/helm-values/kafka-values.yaml `
--values ./infrastructure/kafka/helm-values/kafka-disable-seccomp.yaml
```
- For all other platforms install Kafka with

```pwsh
helm upgrade --install kafka bitnami/kafka --version 25.3.0 `
--values ./infrastructure/kafka/helm-values/kafka-values.yaml
```

- Install Kafka-UI

```pwsh
helm upgrade --install kafka-ui kafka-ui/kafka-ui --version 0.7.5 `
--values ./infrastructure/kafka/helm-values/kafka-ui-values.yaml
```

Check pods are running:

```pwsh
kubectl get pods
```

Example output:

```
NAME                        READY   STATUS    RESTARTS   AGE
kafka-controller-0          0/1     Running   0          79s
kafka-ui-6b8668d4db-bkffr   0/1     Running   0          49s
```

If you need to connect to Kafka from outside cluster you can run the following to enable access to it on `kafka.k8s.tbr.localhost:30094`
```pwsh
helm upgrade --install kafka bitnami/kafka --version 25.3.0 `
--values ./infrastructure/kafka/helm-values/kafka-externalaccess.yaml `
--reuse-values
```

Test connecting to Kafka-UI using browser at `http://kafkaui.k8s.tbr.localhost/`.
