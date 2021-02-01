#See https://aka.ms/containerfastmode to understand how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/core/aspnet:3.1-buster-slim AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/core/sdk:3.1-buster AS build
WORKDIR /src
COPY ["src/RetailInMotion.WebApi/RetailInMotion.WebApi.csproj", "src/RetailInMotion.WebApi/"]
COPY ["src/RetailInMotion.Model/RetailInMotion.Model.csproj", "src/RetailInMotion.Model/"]
COPY ["src/RetailInMotion.Publisher/RetailInMotion.Publisher.csproj", "src/RetailInMotion.Publisher/"]
COPY ["Shared/Shared.MessageBus/Shared.MessageBus.csproj", "Shared/Shared.MessageBus/"]
COPY ["Shared/Shared.Utils/Shared.Utils.csproj", "Shared/Shared.Utils/"]

RUN dotnet restore "src/RetailInMotion.WebApi/RetailInMotion.WebApi.csproj"
COPY . .
WORKDIR "/src/src/RetailInMotion.WebApi"
RUN dotnet build "RetailInMotion.WebApi.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "RetailInMotion.WebApi.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "RetailInMotion.WebApi.dll"]