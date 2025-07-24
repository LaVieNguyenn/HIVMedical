FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

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
ENTRYPOINT ["dotnet", "ApiGateway.dll"] 