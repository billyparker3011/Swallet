using HnMicro.Framework.Helpers;
using HnMicro.Module.Caching.ByRedis.Helpers;
using Lottery.Core.Helpers;
using Lottery.Player.OddsService.Helpers;
using Lottery.Player.OddsService.Hubs;

var builder = WebApplication.CreateBuilder(args);

builder.BuildSignalRService();
builder.BuildLotteryService();
builder.BuildRedis();
builder.BuildOddsService();

var app = builder.Build();

app.MapHub<OddsHub>("/odds");

app.Run();
