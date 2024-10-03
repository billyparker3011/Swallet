FROM mcr.microsoft.com/dotnet/aspnet:7.0 AS base
WORKDIR /app
EXPOSE 80

FROM mcr.microsoft.com/dotnet/sdk:7.0 AS build
WORKDIR /src
COPY . .
RUN dotnet restore Projects/Lottery/Agent/AgentService/Lottery.Agent.AgentService/Lottery.Agent.AgentService.csproj
RUN dotnet build Projects/Lottery/Agent/AgentService/Lottery.Agent.AgentService/Lottery.Agent.AgentService.csproj -c Release -o /app/build

FROM build AS publish
RUN dotnet publish Projects/Lottery/Agent/AgentService/Lottery.Agent.AgentService/Lottery.Agent.AgentService.csproj -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Lottery.Agent.AgentService.dll"]
