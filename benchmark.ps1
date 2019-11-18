#!/usr/bin/env pwsh

docker-compose build;
docker-compose up;
docker-compose down;
docker-compose run mmq-load-tests -- status.sh