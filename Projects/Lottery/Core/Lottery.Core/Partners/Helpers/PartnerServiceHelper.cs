using HnMicro.Module.Caching.ByRedis.Helpers;
using HnMicro.Modules.EntityFrameworkCore.SqlServer;
using Lottery.Core.Partners.Publish;
using Lottery.Core.Partners.Subscriber;
using Lottery.Core.UnitOfWorks;
using Lottery.Data;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Lottery.Core.Partners.Helpers
{
    public static class PartnerServiceHelper
    {
        public static void BuildPartnerService(this IServiceCollection serviceCollection, IConfigurationRoot configurationRoot)
        {
            serviceCollection.AddSqlServer<LotteryContext>(configurationRoot);
            serviceCollection.AddScoped<ILotteryUow, LotteryUow>();
            serviceCollection.AddRedis(configurationRoot);
            serviceCollection.AddSingleton<IPartnerSubscribeService, PartnerSubscribeService>();
            serviceCollection.AddTransient<IPartnerPublish, PartnerPublish>();
        }
    }
}
