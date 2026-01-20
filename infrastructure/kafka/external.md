# External Kafka
> [!IMPORTANT]
> The instructions in this section are intended for sandbox/dev/test environment only<br/>
> Use `oc` command instead of `kubectl` command below when using OpenShift<br/>

If using Kafka outside of cluster then adjust environment variables in:
- [`./init-configs`](./init-configs)

**Note** that you may also need to update some of the environment variables used in Helm chart values:
 - [`../../meniga/meniga-hub/meniga-hub-values.yaml`](../../meniga/meniga-hub/meniga-hub-values.yaml)
 - [`../../meniga/cashflow-assistant/cashflow-assistant-values.yaml`](../../meniga/cashflow-assistant/cashflow-assistant-values.yaml)
 - [`../../meniga/insights-factory/insights-factory-values.yaml`](../../meniga/insights-factory/insights-factory-values.yaml)

Build and apply the YAML for Kafka

```pwsh
kubectl kustomize --load-restrictor=LoadRestrictionsNone ./infrastructure/kafka/init-configs | kubectl apply -f $_ -
```

## References
- [Kafka documentation](https://kafka.apache.org/24/documentation.html)
