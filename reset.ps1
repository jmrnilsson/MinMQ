Set-Location .\MinMQ
docker-compose.exe down
docker volume rm minmq_postgresdata
docker-compose.exe up mmq-db