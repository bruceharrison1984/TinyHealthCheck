#See https://aka.ms/containerfastmode to understand how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/runtime:7.0 AS base
WORKDIR /app

FROM mcr.microsoft.com/dotnet/sdk:7.0 AS build
WORKDIR /src
COPY ["DummyServiceWorker/DummyServiceWorker.csproj", "DummyServiceWorker/"]
COPY ["TinyHealthCheck/TinyHealthCheck.csproj", "TinyHealthCheck/"]
RUN dotnet restore "DummyServiceWorker/DummyServiceWorker.csproj"
COPY . .
WORKDIR "/src/DummyServiceWorker"
RUN dotnet build "DummyServiceWorker.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "DummyServiceWorker.csproj" -c Release -o /app/publish

FROM base AS final

EXPOSE 8080
EXPOSE 8081
EXPOSE 8082

WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "DummyServiceWorker.dll"]