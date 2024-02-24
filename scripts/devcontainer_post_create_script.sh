#!/bin/bash

# Trust the HTTPS development certificate
dotnet dev-certs https --trust

# Install dotnet-ef tool globally
dotnet tool install --global dotnet-ef

# Install dotnet-reportgenerator-globaltool globally
dotnet tool install --global dotnet-reportgenerator-globaltool
