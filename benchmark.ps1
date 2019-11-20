#!/usr/bin/env pwsh

docker-compose build;
docker-compose down;
docker-compose up -d;
docker-compose run mmq-benchmark -- status.sh
docker-compose run mmq-benchmark -- post_message.sh