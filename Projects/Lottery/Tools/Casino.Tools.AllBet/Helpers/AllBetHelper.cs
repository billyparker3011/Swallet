using Casino.Tools.AllBet.Services.Periodic;
using Lottery.Core.Contexts;
using Lottery.Core.Partners;
using Lottery.Core.Partners.Casino;
using Lottery.Core.Partners.CockFight.GA28;
using Lottery.Core.Partners.Periodic;
using Lottery.Core.Services.Partners.CA;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Casino.Tools.AllBet.Helpers
{
    public static class AllBetHelper
    {
        public static void BuildCasinoService(this IServiceCollection serviceCollection, IConfigurationRoot configurationRoot)
        {
            serviceCollection.AddSingleton<IPeriodicService, AllBetPeriodicService>();
            serviceCollection.AddScoped<IPartnerService, CasinoAlibetService>();
            serviceCollection.AddScoped<ICasinoService, CasinoService>();
            serviceCollection.AddSingleton<ILotteryClientContext, LotteryClientContext>();
        }
    }
}
