# Install RabbitMQ

> [!IMPORTANT]
> The instructions in this section are intended for sandbox/dev/test environment only<br/>
> Use `oc` command instead of `kubectl` command below when using OpenShift<br/>
> If using external RabbitMQ go to [External RabbitMQ](./external.md)

When deploying addtional modules Kafka or Rabbit MQ are required. The following assumes RabbitMQ is used.

- Install RabbitMQ Operator
For more information on options for deploying RabbitMQ on Kubernetes [see](https://www.rabbitmq.com/kubernetes/operator/install-operator)

```
kubectl apply -f "https://github.com/rabbitmq/cluster-operator/releases/latest/download/cluster-operator.yml"
```

- Install RabbitMQ Single Node Cluster

```
kubectl apply -f ./infrastructure/rabbitmq/rabbitmq.yaml
```

Check all pods are ready

```pwsh
kubectl get pods
```

Example output:

```
NAME                                              READY   STATUS        RESTARTS   AGE
rmq-server-0                                      1/1     Running       0          4h11m
```

- Build and apply the YAML for RabbitMQ

```
kubectl kustomize --load-restrictor=LoadRestrictionsNone ./infrastructure/rabbitmq/init-configs | kubectl apply -f $_ -
```
