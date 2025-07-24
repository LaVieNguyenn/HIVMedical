FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
WORKDIR /app
EXPOSE 5190

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src
COPY ["HivMedical.Gateway/ApiGateway/ApiGateway.csproj", "HivMedical.Gateway/ApiGateway/"]
RUN dotnet restore "HivMedical.Gateway/ApiGateway/ApiGateway.csproj"
COPY ["HivMedical.Gateway/", "HivMedical.Gateway/"]
WORKDIR "/src/HivMedical.Gateway/ApiGateway"
RUN dotnet build "ApiGateway.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "ApiGateway.csproj" -c Release -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
COPY ["HivMedical.Gateway/ApiGateway/appsettings.Docker.json", "./appsettings.json"]
COPY ["HivMedical.Gateway/ApiGateway/ocelot.Docker.json", "./ocelot.json"]

# Set environment to Development and configure URLs
ENV ASPNETCORE_ENVIRONMENT=Development
ENV ASPNETCORE_URLS=http://+:5190
# Disable HTTPS redirection in Docker
ENV ASPNETCORE_HTTPS_PORT=""

ENTRYPOINT ["dotnet", "ApiGateway.dll"]
