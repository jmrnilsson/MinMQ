#!/usr/bin/env pwsh
docker volume create --name=fasterdbo --driver local --opt o=size=2200m --opt device="K://docker-hlog-1337h" --opt type=tmpfs