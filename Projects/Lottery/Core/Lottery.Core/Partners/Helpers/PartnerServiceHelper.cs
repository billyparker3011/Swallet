using HnMicro.Module.Caching.ByRedis.Helpers;
using HnMicro.Modules.EntityFrameworkCore.SqlServer;
using HnMicro.Modules.InMemory.Contexts;
using HnMicro.Modules.InMemory.UnitOfWorks;
using Lottery.Core.Partners.Enums;
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
            serviceCollection.AddSingleton<IInMemoryContext, InMemoryContext>();
            serviceCollection.AddSingleton<IInMemoryUnitOfWork, InMemoryUnitOfWork>();
            serviceCollection.AddSqlServer<LotteryContext>(configuration);
            serviceCollection.AddScoped<ILotteryUow, LotteryUow>();
            serviceCollection.AddRedis(configuration);
            serviceCollection.AddSingleton<IPartnerSubscribeService, PartnerSubscribeService>();
            //serviceCollection.AddTransient<IPartnerPublishService, PartnerPublishService>();
            serviceCollection.AddHostedService<StartPeriodicService>();
            serviceCollection.AddHostedService<StartScanTicketPeriodicService>();
        }

        public static CockFightSelection ToCockFightSelection(this string selection)
        {
            selection = string.IsNullOrEmpty(selection) ? string.Empty : selection.Trim().ToLower();
            if (selection == "player") return CockFightSelection.Player;
            if (selection == "banker") return CockFightSelection.Banker;
            if (selection == "draw") return CockFightSelection.Draw;
            return CockFightSelection.Unknown;
        }
    }
}
