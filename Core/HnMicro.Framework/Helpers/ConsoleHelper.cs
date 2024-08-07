using HnMicro.Framework.Exceptions;
using HnMicro.Framework.Options;
using HnMicro.Framework.Services;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace HnMicro.Framework.Helpers
{
    public static class ConsoleHelper
    {
        public static IConfigurationRoot CreateConfigurationRoot()
        {
            return new ConfigurationBuilder()
                .AddJsonFile("appsettings.json", false)
                .Build();
        }

        public static IServiceCollection CreateServiceCollection(this IConfigurationRoot configurationRoot)
        {
            var serviceCollection = new ServiceCollection();
            serviceCollection.AddLogging(loggerBuilder =>
            {
                loggerBuilder.ClearProviders();
                loggerBuilder.AddSimpleConsole(option =>
                {
                    option.UseUtcTimestamp = true;
                    option.TimestampFormat = "[MM/dd/yyyy HH:mm:ss] ";
                });
            });
            serviceCollection.AddSingleton<IConfiguration>(configurationRoot);
            serviceCollection.AddSingleton(configurationRoot);
            serviceCollection.AddSingleton<IClockService, ClockService>();
            serviceCollection.AddHttpContextAccessor();
            serviceCollection.AddHttpClient();

            var serviceOption = configurationRoot.GetSection(ServiceOption.AppSettingName).Get<ServiceOption>();
            if (serviceOption == null) throw new HnMicroException($"Cannot find {ServiceOption.AppSettingName} service.");
            serviceCollection.AddSingleton(serviceOption);

            return serviceCollection;
        }
    }
}
