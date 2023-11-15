# OStats.dev API

## Running tests

Run tests and collect coverage results
```
dotnet test src/ --collect:"XPlat Code Coverage" --results-directory:".coverage_report"
```

Generate report
```
reportgenerator -reports:"OStats.Tests/TestResults/{Guid}/coverage.cobertura.xml" -targetdir:"../.coverage_report" -reporttypes:Html
```

## Database migration

Generate migration
```
dotnet ef migrations add <MIGRATION_NAME> --project src/OStats.Infrastructure/
```

Generate idempotent script
```
dotnet ef migrations script --idempotent --output=./migration.sql --project src/OStats.Infrastructure/
```

Database port-forward
```
doctl auth init
doctl kubernetes cluster kubeconfig save 2d90a1ff-1ea2-4c58-b8a7-5f6e66fac1ec
kubectl port-forward service/database 5432:5432 --namespace=ostats
```