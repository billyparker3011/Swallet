using HnMicro.Framework.Data.Models;
using HnMicro.Framework.Helpers;
using Microsoft.AspNetCore.Builder;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace HnMicro.Modules.EntityFrameworkCore.SqlServer
{
    public static class DbContextHelper
    {
        public static string GetConnectionString()
        {
            var configuration = Directory.GetCurrentDirectory().BuildConfiguration();
            var sqlServerOption = configuration.GetSection(SqlConnectionOption.AppSettingName).Get<SqlConnectionOption>();
            if (sqlServerOption == null)
            {
                throw new ArgumentNullException(nameof(SqlConnectionOption));
            }
            return sqlServerOption.Connection;
        }

        public static void AddSqlServer<T>(this WebApplicationBuilder builder) where T : DbContext
        {
            var dbConnection = builder.Configuration.GetSection(SqlConnectionOption.AppSettingName).Get<SqlConnectionOption>() ?? throw new ArgumentNullException(nameof(SqlConnectionOption));
            if (dbConnection.UsePool)
            {
                builder.Services.AddDbContextPool<T>(options => options.UseSqlServer(dbConnection.Connection));
            }
            else
            {
                builder.Services.AddDbContext<T>(options => options.UseSqlServer(dbConnection.Connection));
            }
        }

        public static void AddSqlServer<T>(this IServiceCollection serviceCollection, IConfigurationRoot configurationRoot) where T : DbContext
        {
            var dbConnection = configurationRoot.GetSection(SqlConnectionOption.AppSettingName).Get<SqlConnectionOption>() ?? throw new ArgumentNullException(nameof(SqlConnectionOption));
            if (dbConnection.UsePool)
            {
                serviceCollection.AddDbContextPool<T>(options => options.UseSqlServer(dbConnection.Connection));
            }
            else
            {
                serviceCollection.AddDbContext<T>(options => options.UseSqlServer(dbConnection.Connection));
            }
        }
    }
}
