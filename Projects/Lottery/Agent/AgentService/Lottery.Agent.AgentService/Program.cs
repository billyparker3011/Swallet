using HnMicro.Framework.Helpers;
using HnMicro.Module.Caching.ByRedis.Helpers;
using HnMicro.Modules.EntityFrameworkCore.Helpers;
using Lottery.Core.Helpers;
using Lottery.Data;

var builder = WebApplication.CreateBuilder(args);

builder.BuildServices();
builder.BuildLotteryService();
builder.BuildRedis();

var app = builder.Build();

app.Migrate<LotteryContext>();
app.Usage();
app.Run();
