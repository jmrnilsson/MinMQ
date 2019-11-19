#!/usr/bin/env pwsh

docker-compose build;
docker-compose down;
docker-compose up -d;
docker-compose run mmq-load-tests -- status.sh
docker-compose run mmq-load-tests -- status.sh