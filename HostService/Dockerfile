#See https://aka.ms/customizecontainer to learn how to customize your debug container and how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/aspnet:8.0.0 AS base
USER app
WORKDIR /app

FROM --platform=$BUILDPLATFORM mcr.microsoft.com/dotnet/sdk:8.0.100-1-bookworm-slim AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src
COPY ["HostService/HostService.csproj", "HostService/"]
COPY ["Auth.Database/Auth.Database.csproj", "Auth.Database/"]
COPY ["Crosscutting/Crosscutting.csproj", "Crosscutting/"]
COPY ["DiscordSaga.Components/DiscordSaga.Components.csproj", "DiscordSaga.Components/"]
COPY ["Database.Infrastructure/Database.Infrastructure.csproj", "Database.Infrastructure/"]
COPY ["BoundBot.Connection/BoundBot.Connection.csproj", "BoundBot.Connection/"]
COPY ["Database.Application/Database.Application.csproj", "Database.Application/"]
COPY ["DiscordBot.Application/DiscordBot.Application.csproj", "DiscordBot.Application/"]
COPY ["DiscordBot.Domain/DiscordBot.Domain.csproj", "DiscordBot.Domain/"]
COPY ["Sellix.Application/Sellix.Application.csproj", "Sellix.Application/"]
COPY ["DiscordBot.Infrastructure/DiscordBot.Infrastructure.csproj", "DiscordBot.Infrastructure/"]
COPY ["BoundBot.Components/BoundBot.Components.csproj", "BoundBot.Components/"]
COPY ["LoggingService.Components/LoggingService.Components.csproj", "LoggingService.Components/"]
RUN dotnet restore "./HostService/./HostService.csproj" 
COPY . .
WORKDIR "/src/HostService"
RUN dotnet build "./HostService.csproj" -c $BUILD_CONFIGURATION -o /app/build

FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "./HostService.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "HostService.dll"]