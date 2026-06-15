
FROM mcr.microsoft.com/dotnet/aspnet:10.0-alpine AS base
WORKDIR /app
EXPOSE 8080

# Bibliotecas nativas essenciais para o driver da Oracle rodar estável no Alpine
RUN apk add --no-cache libssl3 libstdc++ libgcc krb5-libs

ENV DOTNET_SYSTEM_GLOBALIZATION_INVARIANT=true

FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src

COPY ["Users.API/Users.API.csproj", "Users.API/"]
COPY ["Users.Domain/Users.Domain.csproj", "Users.Domain/"]
COPY ["Users.Infra.Data/Users.Infra.Data.csproj", "Users.Infra.Data/"]
COPY ["Users.Infra.CrossCutting/Users.Infra.CrossCutting.csproj", "Users.Infra.CrossCutting/"]

RUN dotnet restore "Users.API/Users.API.csproj"

COPY . .
WORKDIR "/src/Users.API"

RUN dotnet publish "Users.API.csproj" \
    -c $BUILD_CONFIGURATION \
    -o /app/publish \
    /p:UseAppHost=false \
    --no-restore

FROM base AS final
WORKDIR /app
COPY --from=build /app/publish .
ENTRYPOINT ["dotnet", "Users.API.dll"]