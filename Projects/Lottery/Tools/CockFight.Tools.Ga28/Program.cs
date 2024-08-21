using CockFight.Tools.Ga28.Helpers;
using HnMicro.Framework.Helpers;
using Lottery.Core.Partners.Helpers;
using Microsoft.Extensions.Hosting;

namespace CockFight.Tools.Ga28
{
    public class Program
    {
        public static async Task Main(string[] args)
        {
            try
            {
                await new HostBuilder()
                    .ConfigureServices((hostContext, services) =>
                    {
                        hostContext.CreateConfigurationRoot();
                        services.AddCoreServices(hostContext.Configuration);
                        services.AddPartnerServices(hostContext.Configuration);
                        services.AddGa28Service();
                    })
                    .UseConsoleLifetime()
                    .RunConsoleAsync();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                Console.WriteLine(ex.StackTrace);
            }
        }
    }
}
