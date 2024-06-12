using HnMicro.Framework.Helpers;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace HnMicro.Framework.UnitTests.Helpers
{
    public static class SpecsHelper
    {
        public static IConfiguration BuildConfiguration(this string appSettings)
        {
            var builder = new ConfigurationBuilder()
                            .AddJsonFile(appSettings, false, true);

            return builder.Build();
        }

        public static IServiceCollection BuildServiceCollection(this IConfiguration configuration, Action<IServiceCollection> action)
        {
            var serviceCollection = new ServiceCollection();
            //serviceCollection.AddCommonService(configuration);
            //action?.Invoke(serviceCollection);
            return serviceCollection;
        }

        public static IServiceCollection BuildServiceCollection(this IConfiguration configuration)
        {
            var serviceCollection = new ServiceCollection();
            //serviceCollection.AddCommonService(configuration);
            return serviceCollection;
        }
    }
}
