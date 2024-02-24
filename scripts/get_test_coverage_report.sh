#!/bin/bash

RESULTS_DIR=".coverage_report"
TEST_OUTPUT_FILE="$RESULTS_DIR/script_test_output.txt"

# Check if results directory exists, if not, create it
if [ ! -d "$RESULTS_DIR" ]; then
  mkdir $RESULTS_DIR
fi

# Run tests
# Exclude DataServiceGrpc (generated code by protoc) and OStats.Infrastructure.Migrations.ContextModelSnapshot (EF Core migration snapshot file)
# If more settings are needed, move them to a .runsettings file and reference it with --settings as per https://github.com/coverlet-coverage/coverlet/blob/master/Documentation/VSTestIntegration.md
echo "Running tests..."
dotnet test src/ \
  --collect:"XPlat Code Coverage;Exclude=[*]DataServiceGrpc*,[*]OStats.Infrastructure.Migrations.ContextModelSnapshot*" \
  --results-directory:$RESULTS_DIR \
  | tee $TEST_OUTPUT_FILE

# Get UUID from last line of test output
run_uuid=$(tail -n 1 $TEST_OUTPUT_FILE | awk -F '.coverage_report/' '{print substr($2, 1, 36)}')
echo "Run UUID: $run_uuid"

# Copy coverage xml to root
echo "Copying coverage xml to root..."
cp "$RESULTS_DIR/$run_uuid/coverage.cobertura.xml" .

# Generate test coverage report
echo "Generating test coverage report..."
reportgenerator -reports:"$RESULTS_DIR/$run_uuid/coverage.cobertura.xml" -targetdir:"$RESULTS_DIR/$run_uuid" -reporttypes:Html 1> /dev/null

# Zip the results
echo "Zipping test coverage results..."
zip -r coverage.zip "$RESULTS_DIR/$run_uuid" 1> /dev/null

echo "Test coverage report zipped to coverage.zip."
