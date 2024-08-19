using Casino.Tools.AllBet.Services.Initial;
using Casino.Tools.AllBet.Services.Periodic;
using Lottery.Core.Partners;
using Lottery.Core.Partners.Casino.Allbet;
using Lottery.Core.Partners.Periodic;
using Microsoft.Extensions.DependencyInjection;

namespace Casino.Tools.AllBet.Helpers
{
    public static class AllBetHelper
    {
        public static void AddCasinoService(this IServiceCollection serviceCollection)
        {
            serviceCollection.AddSingleton<IPeriodicService, AllBetPeriodicService>();
            serviceCollection.AddSingleton<IScanTicketPeriodicService, AllBetScanTicketPeriodicService>();
            serviceCollection.AddScoped<IPartnerService, CasinoAllbetService>();
            serviceCollection.AddHostedService<AllBetInitialService>();
        }
    }
}
