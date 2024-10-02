FROM mcr.microsoft.com/dotnet/aspnet:7.0 AS base
WORKDIR /app
EXPOSE 80

ENV ASPNETCORE_URLS=http://+:80

FROM --platform=$BUILDPLATFORM mcr.microsoft.com/dotnet/sdk:7.0 AS build
ARG configuration=Release
WORKDIR /src
COPY ["Projects/SWallet/Flow/SWallet.ManagerService/SWallet.ManagerService.csproj", "Projects/SWallet/Flow/SWallet.ManagerService/"]
RUN dotnet restore "Projects/SWallet/Flow/SWallet.ManagerService/SWallet.ManagerService.csproj"
COPY . .
WORKDIR "/src/Projects/SWallet/Flow/SWallet.ManagerService"
RUN dotnet build "SWallet.ManagerService.csproj" -c $configuration -o /app/build

FROM build AS publish
ARG configuration=Release
RUN dotnet publish "SWallet.ManagerService.csproj" -c $configuration -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "SWallet.ManagerService.dll"]
