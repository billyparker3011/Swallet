FROM mcr.microsoft.com/dotnet/aspnet:7.0 AS base
WORKDIR /app
EXPOSE 80

FROM mcr.microsoft.com/dotnet/sdk:7.0 AS build
WORKDIR /src
COPY . .
RUN dotnet restore Projects/Lottery/Match/Lottery.Match.MatchService/Lottery.Match.MatchService.csproj
RUN dotnet build Projects/Lottery/Match/Lottery.Match.MatchService/Lottery.Match.MatchService.csproj -c Release -o /app/build

FROM build AS publish
RUN dotnet publish Projects/Lottery/Match/Lottery.Match.MatchService/Lottery.Match.MatchService.csproj -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Lottery.Match.MatchService.dll"]