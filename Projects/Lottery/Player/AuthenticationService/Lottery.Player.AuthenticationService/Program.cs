using HnMicro.Framework.Helpers;
using HnMicro.Module.Caching.ByRedis.Helpers;
using Lottery.Core.Filters;
using Lottery.Core.Helpers;

var builder = WebApplication.CreateBuilder(args);

builder.BuildServices(typeof(PlayerFilter));
builder.BuildLotteryService(false);
builder.BuildRedis();

var app = builder.Build();

app.Usage();
app.Run();