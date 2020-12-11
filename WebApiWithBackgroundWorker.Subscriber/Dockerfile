#See https://aka.ms/containerfastmode to understand how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/aspnet:5.0-buster-slim AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/sdk:5.0-buster-slim AS build
WORKDIR /src
COPY ["WebApiWithBackgroundWorker.Subscriber/WebApiWithBackgroundWorker.Subscriber.csproj", "WebApiWithBackgroundWorker.Subscriber/"]
COPY ["WebApiWithBackgroundWorker.Common/WebApiWithBackgroundWorker.Common.csproj", "WebApiWithBackgroundWorker.Common/"]
RUN dotnet restore "WebApiWithBackgroundWorker.Subscriber/WebApiWithBackgroundWorker.Subscriber.csproj"
COPY . .
WORKDIR "/src/WebApiWithBackgroundWorker.Subscriber"
RUN dotnet build "WebApiWithBackgroundWorker.Subscriber.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "WebApiWithBackgroundWorker.Subscriber.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "WebApiWithBackgroundWorker.Subscriber.dll"]