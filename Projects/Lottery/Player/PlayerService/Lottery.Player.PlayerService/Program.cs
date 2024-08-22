using HnMicro.Framework.Helpers;
using HnMicro.Module.Caching.ByRedis.Helpers;
using HnMicro.Modules.LoggerService.Helpers;
using Lottery.Core.Helpers;
using Lottery.Core.Partners.Attribute.CA;
using Lottery.Core.Partners.Attribute.CockFight;
using Lottery.Player.PlayerService.Helpers;
using Microsoft.AspNetCore.Authentication;

var builder = WebApplication.CreateBuilder(args);

builder.BuildServices();
builder.BuildLotteryService();
builder.BuildInternalPlayerService();
builder.BuildRedis();
builder.BuildClientLoggerService();

builder.Services.AddAuthentication().AddScheme<AuthenticationSchemeOptions, CasinoAuthorizeAttribute>(nameof(CasinoAuthorizeAttribute), null);
builder.Services.AddAuthentication().AddScheme<AuthenticationSchemeOptions, CockFightAuthorizeAttribute>(nameof(CockFightAuthorizeAttribute), null);

var app = builder.Build();

app.Usage();
app.Run();
