FROM microsoft/dotnet:2.1-sdk AS build
WORKDIR /app

# copy csproj and restore as distinct layers
COPY *.sln .
COPY Bol.Api/*.csproj ./Bol.Api/
COPY Bol.Core/*.csproj ./Bol.Core/
COPY Bol.Core.Tests/*.csproj ./Bol.Core.Tests/
COPY neo/neo/*.csproj ./neo/neo/
COPY neo-cli/neo-cli/*.csproj ./neo-cli/neo-cli/
COPY neo-vm/src/neo-vm/*.csproj ./neo-vm/src/neo-vm/

RUN dotnet restore /p:RestoreUseSkipNonexistentTargets="false"

# copy everything else and build app
COPY . .
WORKDIR /app/Bol.Api
RUN dotnet publish -c Release -o out
COPY ./Bol.Api/protocol.internal.json ./out/protocol.json
COPY ./Bol.Api/config.internal.json ./out/config.json

FROM microsoft/dotnet:2.1.3-aspnetcore-runtime AS runtime-base

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
ARG validator_wallet=val1.json

WORKDIR /app
COPY --from=build /app/Bol.Api/out ./
COPY validators/$validator_wallet ./validator.json
RUN mkdir /blockchain
ENTRYPOINT ["dotnet", "Bol.Api.dll"]