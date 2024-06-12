using HnMicro.Framework.Helpers;
using HnMicro.Module.Caching.ByRedis.Helpers;
using HnMicro.Modules.EntityFrameworkCore.Helpers;
using HnMicro.Modules.LoggerService.Helpers;
using Lottery.Core.Filters;
using Lottery.Core.Helpers;
using Lottery.Data;

var builder = WebApplication.CreateBuilder(args);

builder.BuildServices(typeof(AgentFilter));
builder.BuildLotteryService(false);
builder.BuildRedis();
builder.BuildClientLoggerService();

var app = builder.Build();

app.Migrate<LotteryContext>();
app.Usage();
app.Run();
