# Use the official .NET 8 SDK image for building
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

# Copy project files first
COPY HivMedical.PatientService/Patient.Api/Patient.Api.csproj HivMedical.PatientService/Patient.Api/
COPY HivMedical.PatientService/Patient.Application/Patient.Application.csproj HivMedical.PatientService/Patient.Application/
COPY HivMedical.PatientService/Patient.Domain/Patient.Domain.csproj HivMedical.PatientService/Patient.Domain/
COPY HivMedical.PatientService/Patient.Infrastructure/Patient.Infrastructure.csproj HivMedical.PatientService/Patient.Infrastructure/
COPY HivMedical.SharedKernel/SharedKernel/SharedKernel.csproj HivMedical.SharedKernel/SharedKernel/
COPY HivMedical.SharedLibrary/SharedLibrary/SharedLibrary.csproj HivMedical.SharedLibrary/SharedLibrary/

# Restore dependencies for each project individually
RUN dotnet restore HivMedical.PatientService/Patient.Api/Patient.Api.csproj

# Copy all source code
COPY . .

# Build the application
WORKDIR /src/HivMedical.PatientService/Patient.Api
RUN dotnet build -c Release -o /app/build

# Publish the application
RUN dotnet publish -c Release -o /app/publish

# Use the official .NET 8 runtime image
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS runtime
WORKDIR /app

# Copy the published application
COPY --from=build /app/publish .

# Expose port
EXPOSE 5072
# Set environment to Development and configure URLs
ENV ASPNETCORE_ENVIRONMENT=Development
ENV ASPNETCORE_URLS=http://+:5072

# Set entry point
ENTRYPOINT ["dotnet", "Patient.Api.dll"]
