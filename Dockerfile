FROM mcr.microsoft.com/dotnet/aspnet:7.0
WORKDIR /app
COPY ./src/OStats.API/bin/Release/net7.0/linux-x64/publish .
ENTRYPOINT ["dotnet", "OStats.API.dll"]