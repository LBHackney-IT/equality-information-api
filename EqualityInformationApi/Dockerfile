FROM mcr.microsoft.com/dotnet/sdk:8.0

ARG LBHPACKAGESTOKEN
ENV LBHPACKAGESTOKEN=$LBHPACKAGESTOKEN

WORKDIR /app

# Copy csproj and nuget config and restore as distinct layers
COPY ./EqualityInformationApi/EqualityInformationApi.csproj ./
COPY /NuGet.Config /root/.nuget/NuGet/NuGet.Config

RUN dotnet restore ./EqualityInformationApi.csproj

# Copy everything else and build
COPY ./EqualityInformationApi ./
RUN dotnet build -c Release -o out
RUN dotnet publish -c Release -o out

EXPOSE ${PORT:-3000}
CMD ASPNETCORE_URLS=http://+:${PORT:-3000} dotnet ./out/EqualityInformationApi.dll
