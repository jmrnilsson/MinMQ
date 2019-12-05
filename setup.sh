#!/usr/bin/env bash
mkdir -p /data
docker volume create --name=postgresdata --driver local --opt o=size=1200m --opt device="/data/docker-hlog" --opt type=tmpfs
 
