using HnMicro.Framework.Helpers;
using HnMicro.Framework.Logger.Options;
using HnMicro.Modules.EntityFrameworkCore.Helpers;
using HnMicro.Modules.LoggerService.SqlProvider.Data;
using HnMicro.Modules.LoggerService.SqlProvider.Options;
using HnMicro.Modules.LoggerService.SqlProvider.UnitOfWorks;
using Microsoft.AspNetCore.Builder;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace HnMicro.Modules.LoggerService.SqlProvider.Helpers
{
    public static class LoggerSqlProviderHelper
    {
        public static string GetAppSettingName()
        {
            return $"{LoggingOption.AppSettingName}:{SqlProviderOption.AppSettingName}";
        }

        public static void BuildSqlProvider(this WebApplicationBuilder builder)
        {
            var sqlProviderOption = builder.Configuration.GetSection(GetAppSettingName()).Get<SqlProviderOption>();
            if (sqlProviderOption == null)
            {
                throw new ArgumentNullException(nameof(SqlProviderOption));
            }

            if (sqlProviderOption.ConnectionStrings.UsePool)
            {
                builder.Services.AddDbContextPool<LoggerContext>(options => options.UseSqlServer(sqlProviderOption.ConnectionStrings.Connection));
            }
            else
            {
                builder.Services.AddDbContext<LoggerContext>(options => options.UseSqlServer(sqlProviderOption.ConnectionStrings.Connection));
            }

            builder.Services.AddScoped<ISqlProviderUow, SqlProviderUow>();
        }

        public static string GetConnectionStringForLoggerSqlProvider()
        {
            var configuration = Directory.GetCurrentDirectory().BuildConfiguration();
            var loggerSqlProviderOption = configuration.GetSection(GetAppSettingName()).Get<SqlProviderOption>();
            if (loggerSqlProviderOption == null)
            {
                throw new ArgumentNullException(nameof(SqlProviderOption));
            }
            return loggerSqlProviderOption.ConnectionStrings.Connection;
        }

        public static void SubcribeMigrate(this WebApplication app)
        {
            var sqlProviderOptionName = GetAppSettingName();
            var sqlProviderOption = app.Configuration.GetSection(sqlProviderOptionName).Get<SqlProviderOption>();
            if (sqlProviderOption != null && sqlProviderOption.Enabled)
            {
                app.Migrate<LoggerContext>();
            }
        }
    }
}
