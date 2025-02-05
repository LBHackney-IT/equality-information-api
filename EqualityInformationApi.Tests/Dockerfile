FROM mcr.microsoft.com/dotnet/sdk:8.0

# disable microsoft telematry
ENV DOTNET_CLI_TELEMETRY_OPTOUT='true'

ARG LBHPACKAGESTOKEN
ENV LBHPACKAGESTOKEN=$LBHPACKAGESTOKEN

ARG SONAR_TOKEN
ENV SONAR_TOKEN=$SONAR_TOKEN

ENV DynamoDb_LocalMode='true'

WORKDIR /app

# Install and run sonar cloud scanner
RUN apt-get update && apt-get install -y openjdk-17-jdk && apt-get clean
RUN dotnet tool install --global dotnet-sonarscanner
ENV PATH="$PATH:/root/.dotnet/tools"

RUN dotnet sonarscanner begin /k:"LBHackney-IT_equality-information-api" /o:"lbhackney-it" /d:sonar.host.url=https://sonarcloud.io /d:sonar.login="${SONAR_TOKEN}"

# Copy csproj and nuget config and restore as distinct layers
COPY ./EqualityInformationApi.sln ./
COPY ./EqualityInformationApi/EqualityInformationApi.csproj ./EqualityInformationApi/
COPY ./EqualityInformationApi.Tests/EqualityInformationApi.Tests.csproj ./EqualityInformationApi.Tests/
COPY /NuGet.Config /root/.nuget/NuGet/NuGet.Config

RUN dotnet restore ./EqualityInformationApi/EqualityInformationApi.csproj
RUN dotnet restore ./EqualityInformationApi.Tests/EqualityInformationApi.Tests.csproj

# Copy everything else and build
COPY . .

RUN dotnet build -c Release -o out EqualityInformationApi/EqualityInformationApi.csproj
RUN dotnet build -c debug -o out EqualityInformationApi.Tests/EqualityInformationApi.Tests.csproj

CMD dotnet test
