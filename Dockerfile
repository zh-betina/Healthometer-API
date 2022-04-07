﻿FROM mcr.microsoft.com/dotnet/aspnet:6.0 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build
WORKDIR /src
COPY ["Healtometer-API/Healtometer-API.csproj", "Healtometer-API/"]
RUN dotnet restore "Healtometer-API/Healtometer-API.csproj"
COPY . .
WORKDIR "/src/Healtometer-API"
RUN dotnet build "Healtometer-API.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "Healtometer-API.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Healtometer-API.dll"]
