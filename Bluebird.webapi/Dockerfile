#See https://aka.ms/containerfastmode to understand how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/core/aspnet:3.1-buster-slim AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/core/sdk:3.1-buster AS build
WORKDIR /src
COPY ["Bluebird.webapi/Bluebird.webapi.csproj", "Bluebird.webapi/"]
RUN dotnet restore "Bluebird.webapi/Bluebird.webapi.csproj"
COPY . .
WORKDIR "/src/Bluebird.webapi"
RUN dotnet build "Bluebird.webapi.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "Bluebird.webapi.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Bluebird.webapi.dll"]