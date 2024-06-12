FROM mcr.microsoft.com/dotnet/aspnet:7.0 AS base
WORKDIR /app
EXPOSE 80

FROM mcr.microsoft.com/dotnet/sdk:7.0 AS build
WORKDIR /src
COPY . .
RUN dotnet restore Projects/Lottery/Ticket/Lottery.Ticket.TicketService/Lottery.Ticket.TicketService.csproj
RUN dotnet build Projects/Lottery/Ticket/Lottery.Ticket.TicketService/Lottery.Ticket.TicketService.csproj -c Release -o /app/build

FROM build AS publish
RUN dotnet publish Projects/Lottery/Ticket/Lottery.Ticket.TicketService/Lottery.Ticket.TicketService.csproj -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Lottery.Ticket.TicketService.dll"]