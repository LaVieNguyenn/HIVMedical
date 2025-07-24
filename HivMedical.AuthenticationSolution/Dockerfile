# Use the official .NET 8 SDK image for building
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

# Copy project files first
COPY HivMedical.AuthenticationSolution/Auth.Api/Auth.Api.csproj HivMedical.AuthenticationSolution/Auth.Api/
COPY HivMedical.AuthenticationSolution/Authentication.Application/Authentication.Application.csproj HivMedical.AuthenticationSolution/Authentication.Application/
COPY HivMedical.AuthenticationSolution/Authentication.Domain/Authentication.Domain.csproj HivMedical.AuthenticationSolution/Authentication.Domain/
COPY HivMedical.AuthenticationSolution/Authentication.Infrastructure/Authentication.Infrastructure.csproj HivMedical.AuthenticationSolution/Authentication.Infrastructure/
COPY HivMedical.SharedKernel/SharedKernel/SharedKernel.csproj HivMedical.SharedKernel/SharedKernel/
COPY HivMedical.SharedLibrary/SharedLibrary/SharedLibrary.csproj HivMedical.SharedLibrary/SharedLibrary/

# Restore dependencies for each project individually
RUN dotnet restore HivMedical.AuthenticationSolution/Auth.Api/Auth.Api.csproj

# Copy all source code
COPY . .

# Build the application
WORKDIR /src/HivMedical.AuthenticationSolution/Auth.Api
RUN dotnet build -c Release -o /app/build

# Publish the application
RUN dotnet publish -c Release -o /app/publish

# Use the official .NET 8 runtime image
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS runtime
WORKDIR /app

# Copy the published application
COPY --from=build /app/publish .

# Expose port
EXPOSE 5051

# Set environment to Development and configure URLs
ENV ASPNETCORE_ENVIRONMENT=Development
ENV ASPNETCORE_URLS=http://+:5051

# Set entry point
ENTRYPOINT ["dotnet", "Auth.Api.dll"]
