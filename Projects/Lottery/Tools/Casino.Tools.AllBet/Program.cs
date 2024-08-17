using Casino.Tools.AllBet.Helpers;
using HnMicro.Framework.Helpers;
using Lottery.Core.Partners.Helpers;
using Microsoft.Extensions.Hosting;

public class Program
{
    public static async Task Main(string[] args)
    {
        await new HostBuilder()
            .ConfigureServices((hostContext, services) =>
            {
                hostContext.CreateConfigurationRoot();
                services.AddCoreServices(hostContext.Configuration);
                services.AddPartnerServices(hostContext.Configuration);
                services.AddCasinoService();
            })
            .UseConsoleLifetime()
            .RunConsoleAsync();
    }
}