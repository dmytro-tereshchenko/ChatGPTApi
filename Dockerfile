#See https://aka.ms/customizecontainer to learn how to customize your debug container and how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
USER app
WORKDIR /app
EXPOSE 8080

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src
COPY . .
RUN dotnet restore "./src/AppSec.AIPromtInjection.WebApi/AppSec.AIPromtInjection.WebApi.csproj"
WORKDIR "/src/src/AppSec.AIPromtInjection.WebApi"
RUN dotnet build "AppSec.AIPromtInjection.WebApi.csproj" -c $BUILD_CONFIGURATION -o /app/build

FROM build AS publish
WORKDIR "/src/src/AppSec.AIPromtInjection.WebApi"
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "AppSec.AIPromtInjection.WebApi.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "AppSec.AIPromtInjection.WebApi.dll"]