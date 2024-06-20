using HnMicro.Framework.Helpers;
using HnMicro.LoggerService.Services;
using HnMicro.Module.Caching.ByRedis.Helpers;
using HnMicro.Modules.LoggerService.Helpers;
using HnMicro.Modules.LoggerService.Services;
using HnMicro.Modules.LoggerService.SqlProvider.Helpers;

namespace HnMicro.LoggerService.Helpers
{
    public static class LoggerServiceHelper
    {
        public static void BuildLoggerService(this WebApplicationBuilder builder)
        {
            builder.Services.AddHealthChecks();
            builder.Services.AddDependencies();
            builder.BuildRedis();
            builder.Services.AddSingleton<ISubscribeLoggerUsingRedisService, SubscribeLoggerUsingRedisService>();   //  We use this module in HostedService, we cannot automatic register to services.
            builder.Services.AddHostedService<InitialService>();
            builder.BuildSubcribeLoggerService();
        }

        public static void Migrate(this WebApplication app)
        {
            app.SubcribeMigrate();
        }
    }
}
