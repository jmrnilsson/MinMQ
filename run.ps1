#!/usr/bin/env pwsh

docker-compose.exe build
docker-compose.exe down
docker-compose.exe run mmq-load-tests -- status.sh