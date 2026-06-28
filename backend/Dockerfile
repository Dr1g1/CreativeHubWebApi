FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
WORKDIR /src
COPY CreativeHubWebApp.sln .
COPY CreativeHubWebApp/CreativeHubWebApp.csproj ./CreativeHubWebApp/
RUN dotnet restore
COPY CreativeHubWebApp/. ./CreativeHubWebApp/
RUN dotnet publish CreativeHubWebApp/CreativeHubWebApp.csproj -c Release -o /app /p:UseAppHost=false

FROM mcr.microsoft.com/dotnet/aspnet:9.0
WORKDIR /app
COPY --from=build /app .
ENV ASPNETCORE_URLS=http://+:8080
EXPOSE 8080
ENTRYPOINT ["dotnet", "CreativeHubWebApp.dll"]