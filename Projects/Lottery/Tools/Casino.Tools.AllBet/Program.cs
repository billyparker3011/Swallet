using Casino.Tools.AllBet.Helpers;
using HnMicro.Framework.Exceptions;
using HnMicro.Framework.Helpers;
using Lottery.Core.Partners.Helpers;
using Lottery.Core.Partners.Periodic;
using Lottery.Core.Partners.Subscriber;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;

public class Program
{
    public static async Task Main(string[] args)
    {
        var configurationRoot = ConsoleHelper.CreateConfigurationRoot();

        var serviceCollection = configurationRoot.CreateServiceCollection();
        serviceCollection.BuildPartnerService(configurationRoot);
        serviceCollection.BuildCasinoService(configurationRoot);

        var serviceProvider = serviceCollection.BuildServiceProvider();

        var partnerChannelService = serviceProvider.GetService<IPartnerSubscribeService>() ?? throw new HnMicroException();
        await partnerChannelService.Subscribe(Lottery.Core.Partners.Configs.PartnerChannelConfigs.AlibetChannel);

        var cAPeriodicServie = serviceProvider.GetService<IPeriodicService>() ?? throw new HnMicroException();
        await cAPeriodicServie.Start();

        while (Console.ReadKey().Key != ConsoleKey.Escape) { }
    }
}