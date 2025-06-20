# Use the official .NET 8 SDK image for building
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

# Copy project files first
COPY HivMedical.PatientService/Patient.Api/Patient.Api.csproj Patient.Api/
COPY HivMedical.PatientService/Patient.Application/Patient.Application.csproj Patient.Application/
COPY HivMedical.PatientService/Patient.Domain/Patient.Domain.csproj Patient.Domain/
COPY HivMedical.PatientService/Patient.Infrastructure/Patient.Infrastructure.csproj Patient.Infrastructure/
COPY HivMedical.SharedKernel/SharedKernel/SharedKernel.csproj HivMedical.SharedKernel/SharedKernel/
COPY HivMedical.SharedLibrary/SharedLibrary/SharedLibrary.csproj HivMedical.SharedLibrary/SharedLibrary/

# Restore dependencies for each project individually
RUN dotnet restore Patient.Api/Patient.Api.csproj

# Copy all source code
COPY HivMedical.PatientService/ .
COPY HivMedical.SharedKernel/ HivMedical.SharedKernel/
COPY HivMedical.SharedLibrary/ HivMedical.SharedLibrary/

# Build the application
WORKDIR /src/Patient.Api
RUN dotnet build -c Release -o /app/build

# Publish the application
RUN dotnet publish -c Release -o /app/publish

# Use the official .NET 8 runtime image
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS runtime
WORKDIR /app

# Copy the published application
COPY --from=build /app/publish .

# Expose port
EXPOSE 8080

# Set entry point
ENTRYPOINT ["dotnet", "Patient.Api.dll"]
