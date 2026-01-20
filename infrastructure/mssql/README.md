# Install Microsoft SQL Server
> [!IMPORTANT]
> The instructions in this section are intended for sandbox/dev/test environment only<br/>
> If using existing SQL Server you can skip this step

## Initialize configs
> [!IMPORTANT]
> Use `oc` command instead of `kubectl` command below when using OpenShift<br/>

- Validate environment variables in [`./init-configs`](./init-configs)

- Create config maps and secrets

```pwsh
kubectl kustomize --load-restrictor=LoadRestrictionsNone ./infrastructure/mssql/init-configs | kubectl apply -f $_ -
```

## Deploy SQL Server container in cluster
> [!IMPORTANT]
> Use `oc` command instead of `kubectl` command below when using OpenShift<br/>

Build and apply the YAML for SQL Server

```pwsh
kubectl kustomize --load-restrictor=LoadRestrictionsNone ./infrastructure/mssql/sql | kubectl apply -f $_ -
```

Wait for SQL server pod to become ready

```pwsh
kubectl get pod -w
```

Example output:
```
NAME      READY   STATUS      RESTARTS   AGE
mssql-0   0/1     Init:0/10   0          6s
mssql-0   0/1     Init:1/10   0          7s
mssql-0   0/1     Init:2/10   0          10s
mssql-0   0/1     Init:3/10   0          13s
mssql-0   0/1     Init:4/10   0          16s
mssql-0   0/1     Init:5/10   0          19s
mssql-0   0/1     Init:6/10   0          23s
mssql-0   0/1     Init:7/10   0          26s
mssql-0   0/1     Init:8/10   0          29s
mssql-0   0/1     Init:9/10   0          32s
mssql-0   0/1     PodInitializing   0          35s
mssql-0   1/1     Running           0          72s
```

Test connecting to database instance on `sql.k8s.tbr.localhost,1433` using e.g. SQL Server Management Studio (SSMS).
