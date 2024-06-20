#See https://aka.ms/customizecontainer to learn how to customize your debug container and how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/aspnet:8.0.5-bookworm-slim AS base
USER app
WORKDIR /app

FROM --platform=$BUILDPLATFORM mcr.microsoft.com/dotnet/sdk:8.0.300-bookworm-slim AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src
COPY ["BoundBot/BoundBot.csproj", "BoundBot/"]
COPY ["BoundBot.Application/BoundBot.Application.csproj", "BoundBot.Application/"]
COPY ["BoundBot.Domain/BoundBot.Domain.csproj", "BoundBot.Domain/"]
COPY ["BoundBot.Components/BoundBot.Components.csproj", "BoundBot.Components/"]
COPY ["BoundBot.Connection/BoundBot.Connection.csproj", "BoundBot.Connection/"]
COPY ["Crosscutting/Crosscutting.csproj", "Crosscutting/"]
COPY ["BoundBot.Infrastructure/BoundBot.Infrastructure.csproj", "BoundBot.Infrastructure/"]
COPY ["Gpt.Components/Gpt.Components.csproj", "Gpt.Components/"]
COPY ["LoggingService.Components/LoggingService.Components.csproj", "LoggingService.Components/"]
RUN dotnet restore "./BoundBot/./BoundBot.csproj" 
COPY . .
WORKDIR "/src/BoundBot"
RUN dotnet build "./BoundBot.csproj" -c $BUILD_CONFIGURATION -o /app/build

FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "./BoundBot.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "BoundBot.dll"]