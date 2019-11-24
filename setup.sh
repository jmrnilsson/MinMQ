#!/usr/bin/env bash
mkdir -p /data
sudo docker volume create --name=fasterdata --driver local --opt o=size=1200m --opt device="/data/docker-hlog" --opt type=tmpfs
