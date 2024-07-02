using HnMicro.Framework.Helpers;
using HnMicro.Module.Caching.ByRedis.Helpers;
using Lottery.Core.Helpers;
using Lottery.Tools.AdjustOddsService.Helpers;

var builder = WebApplication.CreateBuilder(args);

builder.BuildServices();
builder.BuildLotteryService(false);
builder.BuildInternalAdjustOddsService();
builder.BuildRedis();

var app = builder.Build();

app.Usage();
app.Run();