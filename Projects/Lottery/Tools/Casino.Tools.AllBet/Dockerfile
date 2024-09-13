FROM mcr.microsoft.com/dotnet/aspnet:7.0 AS base
WORKDIR /app
EXPOSE 80

FROM mcr.microsoft.com/dotnet/sdk:7.0 AS build
WORKDIR /src
COPY . .
RUN dotnet restore Projects/Lottery/Tools/Casino.Tools.AllBet/Casino.Tools.AllBet.csproj
RUN dotnet build Projects/Lottery/Tools/Casino.Tools.AllBet/Casino.Tools.AllBet.csproj -c Release -o /app/build

FROM build AS publish
RUN dotnet publish Projects/Lottery/Tools/Casino.Tools.AllBet/Casino.Tools.AllBet.csproj -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Casino.Tools.AllBet.dll"]