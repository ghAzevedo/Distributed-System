#See https://aka.ms/containerfastmode to understand how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/core/aspnet:3.1-buster-slim AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/core/sdk:3.1-buster AS build
ENV ASPNETCORE_ENVIRONMENT = "Production"

WORKDIR /src
COPY ["src/RetailInMotion.Order/RetailInMotion.Order.csproj", "src/RetailInMotion.Order/"]
COPY ["src/RetailInMotion.Data/RetailInMotion.Data.csproj", "src/RetailInMotion.Data/"]
COPY ["src/RetailInMotion.Model/RetailInMotion.Model.csproj", "src/RetailInMotion.Model/"]
COPY ["src/RetailInMotion.Order.BackgroundWorkers/RetailInMotion.BackgroundWorkers.csproj", "src/RetailInMotion.Order.BackgroundWorkers/"]
COPY ["src/RetailInMotion.Services/RetailInMotion.Services.csproj", "src/RetailInMotion.Services/"]
COPY ["src/RetailInMotion.ServiceDependencies/RetailInMotion.ServiceDependencies.csproj", "src/RetailInMotion.ServiceDependencies/"]
COPY ["Shared/Shared.Utils/Shared.Utils.csproj", "Shared/Shared.Utils/"]
COPY ["Shared/Shared.MessageBus/Shared.MessageBus.csproj", "Shared/Shared.MessageBus/"]
COPY ["Shared/Shared.Data/Shared.Data.csproj", "Shared/Shared.Data/"]

RUN dotnet restore "src/RetailInMotion.Order/RetailInMotion.Order.csproj"
COPY . .
WORKDIR "/src/src/RetailInMotion.Order"
RUN dotnet build "RetailInMotion.Order.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "RetailInMotion.Order.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "RetailInMotion.Order.dll"]