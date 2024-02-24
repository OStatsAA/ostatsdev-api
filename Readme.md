# OStats.dev API

[![Codacy Badge](https://api.codacy.com/project/badge/Grade/638e5664aebb45af91c65cfbf010dc21)](https://app.codacy.com/gh/OStatsAA/ostatsdev-api?utm_source=github.com&utm_medium=referral&utm_content=OStatsAA/ostatsdev-api&utm_campaign=Badge_Grade)

## Running tests

Run tests and collect coverage results
```
dotnet test src/ --collect:"XPlat Code Coverage" --results-directory:".coverage_report"
```

Generate report
```
reportgenerator -reports:/workspaces/ostatsdev-api/.coverage_report/{Guid}/coverage.cobertura.xml -targetdir:/workspaces/ostatsdev-api/.coverage_report/{Guid} -reporttypes:Html
```

Server HTML report
```
python3 -m http.server --directory .coverage_report/{Guid}/
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
kubectl port-forward service/database 5431:5432 --namespace=ostats
```