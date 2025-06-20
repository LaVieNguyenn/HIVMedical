@echo off
echo.
echo ========================================
echo   HIV Medical Docker Compose Startup
echo ========================================
echo.

echo 🐳 Starting HIV Medical Microservices with Docker...

REM Check if Docker is running
docker version >nul 2>&1
if %errorlevel% neq 0 (
    echo ❌ Docker is not running. Please start Docker Desktop first.
    pause
    exit /b 1
)

echo ✅ Docker is running

echo.
echo 🧹 Cleaning up existing containers...
docker-compose down -v

echo.
echo 🔨 Building and starting services...
docker-compose up --build -d

echo.
echo ⏳ Waiting for services to start...
timeout /t 30 /nobreak >nul

echo.
echo 📊 Checking service status...
docker-compose ps

echo.
echo ✅ Services are starting up!
echo.
echo 🔗 Service URLs:
echo    RabbitMQ Management: http://localhost:15672 (guest/guest)
echo.
echo 📝 Note:
echo - Only RabbitMQ is running in Docker
echo - Database services should be running locally
echo - Auth and Patient services should be run locally using 'dotnet run'
echo.
echo 🔧 Useful commands:
echo    View logs: docker-compose logs -f
echo    Stop services: docker-compose down
echo    Restart services: docker-compose restart
echo.
echo Press any key to exit...
pause >nul
