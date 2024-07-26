using HnMicro.Modules.LoggerService.Helpers;
using Lottery.Player.OddsService.Hubs;
using Lottery.Player.OddsService.Services.Initial;
using Lottery.Player.OddsService.Services.PubSub;

namespace Lottery.Player.OddsService.Helpers
{
    public static class OddsServiceHelper
    {
        public static void BuildOddsService(this WebApplicationBuilder builder)
        {
            builder.Services.AddSingleton<IPingPongService, PingPongService>();
            builder.Services.AddSingleton<IOddsHubHandler, OddsHubHandler>();
            builder.Services.AddSingleton<ISubscribeMatchAndOddsService, SubscribeMatchAndOddsService>();
            builder.Services.AddHostedService<InitialOddsService>();
            builder.BuildClientLoggerService();
        }
    }
}
