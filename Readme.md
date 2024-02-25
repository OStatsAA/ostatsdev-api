# OStats.dev API

[![Codacy Badge](https://app.codacy.com/project/badge/Grade/508ca71a0ad5457e8b5849bd1411f0bf)](https://app.codacy.com/gh/OStatsAA/ostatsdev-api/dashboard?utm_source=gh&utm_medium=referral&utm_content=&utm_campaign=Badge_grade) [![Codacy Badge](https://app.codacy.com/project/badge/Coverage/508ca71a0ad5457e8b5849bd1411f0bf)](https://app.codacy.com/gh/OStatsAA/ostatsdev-api/dashboard?utm_source=gh&utm_medium=referral&utm_content=&utm_campaign=Badge_coverage)

## Running tests

Run tests via IDE or dotnet cli.

```text
dotnet test src/
```

Coverage report may be generated via [Get Coverage Report Script](scripts/get_test_coverage_report.sh)

```text
source scripts/get_test_coverage_report.sh
```

## Database migration

Generate migration

```text
dotnet ef migrations add <MIGRATION_NAME> --project src/OStats.Infrastructure/
```

Generate idempotent script

```text
dotnet ef migrations script --idempotent --output=./migration.sql --project src/OStats.Infrastructure/
```
