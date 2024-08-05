using CockFight.Tools.Ga28.Helpers;
using HnMicro.Framework.Exceptions;
using HnMicro.Framework.Helpers;
using Lottery.Core.Partners.Helpers;
using Lottery.Core.Partners.Periodic;
using Lottery.Core.Partners.Subscriber;
using Microsoft.Extensions.DependencyInjection;

namespace CockFight.Tools.Ga28
{
    public class Program
    {
        public static async Task Main(string[] args)
        {
            var configurationRoot = ConsoleHelper.CreateConfigurationRoot();

            var serviceCollection = configurationRoot.CreateServiceCollection();
            serviceCollection.BuildPartnerService(configurationRoot);
            serviceCollection.BuildGa28Service(configurationRoot);

            var serviceProvider = serviceCollection.BuildServiceProvider();

            var partnerChannelService = serviceProvider.GetService<IPartnerSubscribeService>() ?? throw new HnMicroException();
            await partnerChannelService.Subscribe(Lottery.Core.Partners.Configs.PartnerChannelConfigs.Ga28Channel);

            var ga28PeriodicServie = serviceProvider.GetService<IPeriodicService>() ?? throw new HnMicroException();
            await ga28PeriodicServie.Start();

            while (Console.ReadKey().Key != ConsoleKey.Escape) { }
        }
    }
}
