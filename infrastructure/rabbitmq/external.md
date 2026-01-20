# External RabbitMQ
> [!IMPORTANT]
> The instructions in this section are intended for sandbox/dev/test environment only<br/>
> Use `oc` command instead of `kubectl` command below when using OpenShift<br/>

If using RabbitMQ outside of cluster then adjust the config values in the following files:
- [`./init-configs/core-rabbitmq.env`](./init-configs/core-rabbitmq.env)
- [`./init-configs/cashflow-assistant-rabbitmq.env`](./init-configs/cashflow-assistant-rabbitmq.env)

**Note** that you may also need to update some of the environment variables used in Helm chart values:
- [`../../meniga/meniga-hub/meniga-hub-values.yaml`](../../meniga/meniga-hub/meniga-hub-values.yaml)
- [`../../meniga/cashflow-assistant/cashflow-assistant-values.yaml`](../../meniga/cashflow-assistant/cashflow-assistant-values.yaml)
 - [`../../meniga/insights-factory/insights-factory-values.yaml`](../../meniga/insights-factory/insights-factory-values.yaml)

Build and apply the YAML for RabbitMQ

```
kubectl kustomize --load-restrictor=LoadRestrictionsNone ./infrastructure/rabbitmq/init-configs | kubectl apply -f $_ -
```
