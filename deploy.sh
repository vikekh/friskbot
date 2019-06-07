#!/bin/bash

cd ~/friskbot
docker-compose stop
docker-compose rm -f
docker-compose pull   
docker-compose up -d
