# Voteapp Worker

This Dotnet Core app pulls data from Redis and writes the data to Postgres.  This app is meant to be run on a Windows VM.  This version of the app can replace the dockerized Work app that is part of the Voteapp.

> Note: This application is for demonstration purposes only

## Configure the app to talk to redis and Postgres

The `appsettings.json` file contains two keys (RedisConnectionConfig and DatabaseConnectionConfig).  The values for each of those keys will need to be updated for the app work properly.

> Note: Postgres needs to be a recent version (example: postgres:14.4)

## Install Dotnet Core 6

Start up a Powershell in Windows VM.

    curl https://download.visualstudio.microsoft.com/download/pr/c246f2b8-da39-4b12-b87d-bf89b6b51298/2d43d4ded4b6a0c4d1a0b52f0b9a3b30/dotnet-sdk-6.0.302-win-x64.exe -o dotnet-sdk-6.0.302-win-x64.exe

Run the Dotnet 6 SKD Installer

    .\dotnet-sdk-6.0.302-win-x64.exe

Install dotnet-ef to support database management

    dotnet tool install --global dotnet-ef --version 6.0.7

## Create Postgres 'votes' table

    dotnet ef database update

## Run the Worker as a Windows Service

Commands to start the worker service:
    mkdir "C:\Program Files\worker"
    dotnet publish --output "C:\Program Files\worker\"
    sc.exe create ".NET VoteApp Worker Service" binpath= "C:\Program Files\worker\vote-worker.exe"
    sc.exe start ".NET VoteApp Worker Service"

Commands to remove the worker service:
    sc.exe stop ".NET VoteApp Worker Service"
    sc.exe delete ".NET VoteApp Worker Service"
    rm "C:\Program Files\worker"
