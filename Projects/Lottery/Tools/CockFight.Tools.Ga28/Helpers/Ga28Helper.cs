using CockFight.Tools.Ga28.Services.Periodic;
using Lottery.Core.Partners;
using Lottery.Core.Partners.CockFight.GA28;
using Lottery.Core.Partners.Periodic;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace CockFight.Tools.Ga28.Helpers
{
    public static class Ga28Helper
    {
        public static void BuildGa28Service(this IServiceCollection serviceCollection, IConfigurationRoot configurationRoot)
        {
            serviceCollection.AddSingleton<IPeriodicService, Ga28PeriodicService>();
            serviceCollection.AddScoped<IPartnerService, CockFightGa28Service>();
        }
    }
}
