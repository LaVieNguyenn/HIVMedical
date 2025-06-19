# HivMedical API Gateway

This API Gateway serves as the entry point for all the microservices in the HivMedical system. It routes requests to the appropriate microservices and provides a unified API interface for clients.

## Features

- Centralized routing to microservices
- Swagger UI for API documentation
- Health check endpoint
- CORS support
- Docker support

## Prerequisites

- .NET 7.0 SDK or later
- Docker and Docker Compose (for containerized deployment)
- SQL Server (for the backend services)
- RabbitMQ (for messaging between services)

## Services

The API Gateway routes requests to the following microservices:

1. **Authentication Service** - Handles user authentication and authorization
2. **Appointment Service** - Handles appointment scheduling and management

## How to Run

### Local Development

1. Clone the repository
2. Navigate to the HivMedical.Gateway directory
3. Run the API Gateway:

```bash
cd ApiGateway
dotnet run
```

The API Gateway will be available at:
- HTTP: http://localhost:5000
- HTTPS: https://localhost:5001

### Docker Deployment

To run the entire system using Docker Compose:

```bash
docker-compose up -d
```

This will start:
- SQL Server database
- RabbitMQ message broker
- API Gateway
- Authentication Service

## API Documentation

When running locally, the Swagger UI is available at:

- Gateway Swagger: https://localhost:5001/swagger/index.html
- Auth Service API: https://localhost:5001/swagger/auth/index.html
- Appointment Service API: https://localhost:5001/swagger/appointment/index.html

## Health Check

You can check the gateway status at:

```
GET /api/healthcheck
```

This endpoint returns information about the gateway and its connected services.

## Configuration

The gateway configuration is stored in:
- `appsettings.json` - General application settings
- `ocelot.json` - Routing configuration for the gateway

## Adding New Services

To add a new microservice to the gateway:

1. Add a new route configuration to `ocelot.json`
2. Add the service URL to `appsettings.json`
3. Add the service to `docker-compose.yml` (for containerized deployment) 