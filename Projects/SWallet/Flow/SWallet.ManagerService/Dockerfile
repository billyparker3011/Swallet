FROM mcr.microsoft.com/dotnet/aspnet:7.0 AS base
ENV ASPNETCORE_ENVIRONMENT=Development
WORKDIR /app
EXPOSE 80

FROM mcr.microsoft.com/dotnet/sdk:7.0 AS build
WORKDIR /src
COPY . .
RUN dotnet restore SWallet.ManagerService.csproj
RUN dotnet build Projects/SWallet/Flow/SWallet.ManagerService/SWallet.ManagerService.csproj -c Release -o /app/build

FROM build AS publish
RUN dotnet publish Projects/SWallet/Flow/SWallet.ManagerService/SWallet.ManagerService.csproj -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "SWallet.ManagerService.dll","--environment=Development"]
