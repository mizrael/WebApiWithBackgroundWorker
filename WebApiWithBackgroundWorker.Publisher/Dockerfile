#See https://aka.ms/containerfastmode to understand how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/runtime:5.0-buster-slim AS base
WORKDIR /app

FROM mcr.microsoft.com/dotnet/sdk:5.0-buster-slim AS build
WORKDIR /src
COPY ["WebApiWithBackgroundWorker.Publisher/WebApiWithBackgroundWorker.Publisher.csproj", "WebApiWithBackgroundWorker.Publisher/"]
COPY ["WebApiWithBackgroundWorker.Common/WebApiWithBackgroundWorker.Common.csproj", "WebApiWithBackgroundWorker.Common/"]
RUN dotnet restore "WebApiWithBackgroundWorker.Publisher/WebApiWithBackgroundWorker.Publisher.csproj"
COPY . .
WORKDIR "/src/WebApiWithBackgroundWorker.Publisher"
RUN dotnet build "WebApiWithBackgroundWorker.Publisher.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "WebApiWithBackgroundWorker.Publisher.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "WebApiWithBackgroundWorker.Publisher.dll"]
