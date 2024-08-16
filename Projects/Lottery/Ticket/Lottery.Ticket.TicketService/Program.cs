using HnMicro.Framework.Helpers;
using HnMicro.Module.Caching.ByRedis.Helpers;
using Lottery.Core.Helpers;
using Lottery.Ticket.TicketService.Helpers;

var builder = WebApplication.CreateBuilder(args);

builder.BuildServices();
builder.BuildLotteryService();
builder.BuildRedis();
builder.BuildInternalPlayerService();
builder.BuildBackgroundServices();

var app = builder.Build();

app.Usage();
app.Run();