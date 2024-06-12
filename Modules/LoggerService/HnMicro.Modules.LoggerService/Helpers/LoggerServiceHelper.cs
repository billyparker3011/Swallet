using HnMicro.Modules.LoggerService.Services;
using HnMicro.Modules.LoggerService.SqlProvider.Helpers;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;

namespace HnMicro.Modules.LoggerService.Helpers
{
    public static class LoggerServiceHelper
    {
        public static void BuildClientLoggerService(this WebApplicationBuilder builder)
        {
            builder.Services.AddScoped<ILoggerService, Services.LoggerService>();
        }

        public static void BuildSubcribeLoggerService(this WebApplicationBuilder builder)
        {
            builder.BuildSqlProvider();
        }
    }
}
