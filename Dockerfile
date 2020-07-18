FROM mcr.microsoft.com/dotnet/core/aspnet:3.0-buster-slim AS base

USER root

RUN apt-get update && apt-get install --assume-yes wget

# Pre build commands
RUN wget https://codejudge-starter-repo-artifacts.s3.ap-south-1.amazonaws.com/backend-project/springboot/maven/2.x/pre-build-2.sh
RUN chmod 775 ./pre-build-2.sh
RUN sh pre-build-2.sh

FROM mcr.microsoft.com/dotnet/core/sdk:3.1-buster AS build
COPY ./dotnet3.1-in-docker/*.csproj .
RUN dotnet restore dotnet3.1-in-docker.csproj
COPY ./dotnet3.1-in-docker/ /tmp/

WORKDIR /tmp

RUN dotnet build

EXPOSE 8080

RUN wget https://codejudge-starter-repo-artifacts.s3.ap-south-1.amazonaws.com/backend-project/dotnet/run.sh
RUN chmod 775 ./run.sh
CMD sh run.sh
