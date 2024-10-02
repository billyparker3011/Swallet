FROM mcr.microsoft.com/dotnet/aspnet:7.0 AS base
WORKDIR /app
EXPOSE 80

FROM mcr.microsoft.com/dotnet/sdk:7.0 AS build
WORKDIR /src
COPY . .
RUN dotnet restore Projects/Lottery/Tools/CockFight.Tools.Ga28/CockFight.Tools.Ga28.csproj
RUN dotnet build Projects/Lottery/Tools/CockFight.Tools.Ga28/CockFight.Tools.Ga28.csproj -c Release -o /app/build

FROM build AS publish
RUN dotnet publish Projects/Lottery/Tools/CockFight.Tools.Ga28/CockFight.Tools.Ga28.csproj -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
COPY Projects/Lottery/Tools/CockFight.Tools.Ga28/appsettings.json .

ENTRYPOINT ["dotnet", "CockFight.Tools.Ga28.dll"]