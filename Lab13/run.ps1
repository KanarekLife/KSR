# Requires PowerShell 7+
function Cleanup {
    # Kill processes matching Library.WebApi or Library.Web
    Get-Process | Where-Object { $_.ProcessName -match 'Library\.WebApi|Library\.Web' } | Stop-Process -Force -ErrorAction SilentlyContinue

    # Remove and clean Docker resources
    docker rm -f library-magic-notification-service
    docker rm -f library-magic-rabbitmq
    docker rmi library-magic-notification-service:latest
    docker network rm library-magic-network
}
Cleanup

# Start notification service container
docker run --rm -td -p 9092:80 --name library-magic-notification-service mcr.microsoft.com/dotnet/sdk:8.0-alpine sh -c 'touch /var/log/notification-service.log && tail -f /var/log/notification-service.log'

# Start RabbitMQ container
docker run --rm -d -p 5672:5672 --name library-magic-rabbitmq rabbitmq:4-management-alpine

# Stream logs in background jobs
Start-Job { docker logs -f library-magic-rabbitmq }
Start-Job { docker logs -f library-magic-notification-service }

Start-Sleep -Seconds 3

# Create and connect to Docker network
docker network create library-magic-network
docker network connect library-magic-network library-magic-notification-service
docker network connect library-magic-network library-magic-rabbitmq

# Copy service files into the container
docker cp Library.NotificationService2 library-magic-notification-service:/app
docker exec -it library-magic-notification-service /bin/sh -c "cd /app && dotnet restore && dotnet build -c Release && dotnet publish -c Release -o out"
docker exec -d library-magic-notification-service /bin/sh -c "cd /app/out && RabbitMq__ServerAddress=rabbitmq://library-magic-rabbitmq dotnet Library.NotificationService2.dll > /var/log/notification-service.log"

docker commit library-magic-notification-service library-magic-notification-service:latest
docker image ls | Select-String "library-magic-notification-service"

# Start Library.WebApi locally
Start-Job {
    Set-Location Library.WebApi
    $env:RabbitMq__ServerAddress = "rabbitmq://localhost"
    $env:Kestrel__EndPoints__Http__Url = "http://+:9091"
    dotnet run
}

# Start Library.Web locally
Start-Job {
    Set-Location Library.Web
    $env:LibraryWebApiServiceHost = "http://localhost:9091"
    $env:Kestrel__EndPoints__Http__Url = "http://+:9090"
    dotnet run
}

Write-Host "Press any key to stop the services..." -ForegroundColor Yellow
[void][System.Console]::ReadKey($true)
Cleanup