FROM mcr.microsoft.com/dotnet/aspnet:7.0 AS base
WORKDIR /app
EXPOSE 80

FROM mcr.microsoft.com/dotnet/sdk:7.0 AS build
ARG configuration=Release
WORKDIR /src
COPY ["Projects/SWallet/Flow/SWallet.CustomerService/SWallet.CustomerService.csproj", "Projects/SWallet/Flow/SWallet.CustomerService/"]
RUN dotnet restore "Projects/SWallet/Flow/SWallet.CustomerService/SWallet.CustomerService.csproj"
COPY . .
WORKDIR "/src/Projects/SWallet/Flow/SWallet.CustomerService"
RUN dotnet build "SWallet.CustomerService.csproj" -c $configuration -o /app/build

FROM build AS publish
ARG configuration=Release
RUN dotnet publish "SWallet.CustomerService.csproj" -c $configuration -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "SWallet.CustomerService.dll"]
