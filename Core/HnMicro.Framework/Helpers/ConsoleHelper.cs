using HnMicro.Framework.Exceptions;
using HnMicro.Framework.Options;
using HnMicro.Framework.Services;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace HnMicro.Framework.Helpers
{
    public static class ConsoleHelper
    {
        public static void CreateConfigurationRoot(this HostBuilderContext hostContext)
        {
            hostContext.Configuration = new ConfigurationBuilder().AddJsonFile("appsettings.json", false).Build();
        }

        public static void AddCoreServices(this IServiceCollection serviceCollection, IConfiguration configuration)
        {
            serviceCollection.AddLogging(loggerBuilder =>
            {
                loggerBuilder.ClearProviders();
                loggerBuilder.AddSimpleConsole(option =>
                {
                    option.UseUtcTimestamp = true;
                    option.TimestampFormat = "[MM/dd/yyyy HH:mm:ss] ";
                });
            });
            serviceCollection.AddSingleton(configuration);
            serviceCollection.AddSingleton<IClockService, ClockService>();
            serviceCollection.AddHttpContextAccessor();
            serviceCollection.AddHttpClient();

            var serviceOption = configuration.GetSection(ServiceOption.AppSettingName).Get<ServiceOption>() ?? throw new HnMicroException($"Cannot find {ServiceOption.AppSettingName} service.");
            serviceCollection.AddSingleton(serviceOption);
        }
    }
}
