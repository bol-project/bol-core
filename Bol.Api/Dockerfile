FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build
WORKDIR /app

# copy csproj and restore as distinct layers
COPY *.sln .
COPY Bol.Api/*.csproj ./Bol.Api/
COPY Bol.Core/*.csproj ./Bol.Core/
COPY Bol.Cryptography/*.csproj ./Bol.Cryptography/
COPY Bol.Cryptography.Neo/*.csproj ./Bol.Cryptography.Neo/
COPY Bol.Address/*.csproj ./Bol.Address/
COPY Bol.Address.Neo/*.csproj ./Bol.Address.Neo/
COPY Bol.Core.Tests/*.csproj ./Bol.Core.Tests/
COPY neo/neo/*.csproj ./neo/neo/
COPY neo-cli/neo-cli/*.csproj ./neo-cli/neo-cli/

RUN dotnet restore /p:RestoreUseSkipNonexistentTargets="false"

# copy everything else and build app
COPY . .
WORKDIR /app/Bol.Api
RUN dotnet publish -c Release -o out
COPY ./Bol.Api/protocol.internal.json ./out/protocol.json

FROM mcr.microsoft.com/dotnet/aspnet:6.0 AS runtime-base

# Install dependencies:
RUN apt-get update && apt-get install -y \
    libleveldb-dev \
    sqlite3 \
    libsqlite3-dev \
    libunwind8-dev \
    wget \
    expect \
    screen \
    zip

# APT cleanup to reduce image size
RUN rm -rf /var/lib/apt/lists/*

FROM runtime-base AS runtime

WORKDIR /app
COPY --from=build /app/Bol.Api/out ./
COPY ./validators /validators
RUN mkdir /blockchain
ENTRYPOINT ["dotnet", "Bol.Api.dll"]