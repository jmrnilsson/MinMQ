#!/usr/bin/env pwsh
docker volume create --name=fasterdata --driver local --opt o=size=1200m --opt device="K://docker-hlog" --opt type=tmpfs