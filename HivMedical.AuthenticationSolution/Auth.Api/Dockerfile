FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

# Copy project files
COPY ["Auth.Api/Auth.Api.csproj", "Auth.Api/"]
COPY ["Auth.Application/Auth.Application.csproj", "Auth.Application/"]
COPY ["Auth.Domain/Auth.Domain.csproj", "Auth.Domain/"]
COPY ["Auth.Infrastructure/Auth.Infrastructure.csproj", "Auth.Infrastructure/"]
COPY ["../HivMedical.SharedKernel/SharedKernel/SharedKernel.csproj", "HivMedical.SharedKernel/SharedKernel/"]
COPY ["../HivMedical.SharedLibrary/SharedLibrary/SharedLibrary.csproj", "HivMedical.SharedLibrary/SharedLibrary/"]

# Restore dependencies
RUN dotnet restore "Auth.Api/Auth.Api.csproj"

# Copy the source code
COPY . .
COPY ["../HivMedical.SharedKernel/", "HivMedical.SharedKernel/"]
COPY ["../HivMedical.SharedLibrary/", "HivMedical.SharedLibrary/"]

# Build the API
WORKDIR "/src/Auth.Api"
RUN dotnet build "Auth.Api.csproj" -c Release -o /app/build

# Publish the application
RUN dotnet publish "Auth.Api.csproj" -c Release -o /app/publish

# Build the runtime image
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS runtime
WORKDIR /app
COPY --from=build /app/publish .

# Expose ports
EXPOSE 80
EXPOSE 443

# Set entry point
ENTRYPOINT ["dotnet", "Auth.Api.dll"]
