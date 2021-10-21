FROM mcr.microsoft.com/dotnet/core/sdk:3.1

# disable microsoft telematry
ENV DOTNET_CLI_TELEMETRY_OPTOUT='true'

ARG LBHPACKAGESTOKEN
ENV LBHPACKAGESTOKEN=$LBHPACKAGESTOKEN

WORKDIR /app

# Copy csproj and nuget config and restore as distinct layers
COPY ./EqualityInformationApi.sln ./
COPY ./EqualityInformationApi/EqualityInformationApi.csproj ./EqualityInformationApi/
COPY ./EqualityInformationApi.Tests/EqualityInformationApi.Tests.csproj ./EqualityInformationApi.Tests/
COPY /NuGet.Config /root/.nuget/NuGet/NuGet.Config

RUN dotnet restore ./EqualityInformationApi/EqualityInformationApi.csproj
RUN dotnet restore ./EqualityInformationApi.Tests/EqualityInformationApi.Tests.csproj

# Copy everything else and build
COPY . .

RUN dotnet build -c debug -o out EqualityInformationApi.Tests/EqualityInformationApi.Tests.csproj

CMD dotnet test
