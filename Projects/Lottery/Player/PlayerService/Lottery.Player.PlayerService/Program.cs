using HnMicro.Framework.Helpers;
using HnMicro.Module.Caching.ByRedis.Helpers;
using Lottery.Core.Helpers;
using Lottery.Player.PlayerService.Helpers;

var builder = WebApplication.CreateBuilder(args);

builder.BuildServices();
builder.BuildLotteryService();
builder.BuildInternalPlayerService();
builder.BuildRedis();

var app = builder.Build();

app.Usage();
app.Run();
