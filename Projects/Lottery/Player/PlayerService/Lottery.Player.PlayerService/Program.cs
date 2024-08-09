using HnMicro.Framework.Helpers;
using HnMicro.Module.Caching.ByRedis.Helpers;
using Lottery.Core.Helpers;
using Lottery.Core.Partners.Attribute.CA;
using Lottery.Player.PlayerService.Helpers;
using Microsoft.AspNetCore.Authentication;

var builder = WebApplication.CreateBuilder(args);

builder.BuildServices();
builder.BuildLotteryService();
builder.BuildInternalPlayerService();
builder.BuildRedis();

builder.Services.AddAuthentication().AddScheme<AuthenticationSchemeOptions, CAAuthorizeAttribute>(nameof(CAAuthorizeAttribute), null);

var app = builder.Build();

app.Usage();
app.Run();
