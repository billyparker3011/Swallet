using HnMicro.Module.Caching.ByRedis.Helpers;
using HnMicro.Modules.EntityFrameworkCore.SqlServer;
using Lottery.Core.Partners.Periodic;
using Lottery.Core.Partners.Subscriber;
using Lottery.Core.UnitOfWorks;
using Lottery.Data;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Lottery.Core.Partners.Helpers
{
    public static class PartnerServiceHelper
    {
        public static void AddPartnerServices(this IServiceCollection serviceCollection, IConfiguration configuration)
        {
            serviceCollection.AddSqlServer<LotteryContext>(configuration);
            serviceCollection.AddScoped<ILotteryUow, LotteryUow>();
            serviceCollection.AddRedis(configuration);
            serviceCollection.AddSingleton<IPartnerSubscribeService, PartnerSubscribeService>();
            //serviceCollection.AddTransient<IPartnerPublishService, PartnerPublishService>();
            serviceCollection.AddHostedService<StartPeriodicService>();
        }
    }
}
