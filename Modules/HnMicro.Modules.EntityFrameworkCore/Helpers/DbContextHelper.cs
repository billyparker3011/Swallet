//using HnMicro.Core.Options;
//using HnMicro.Framework.Helpers;
//using HnMicro.Modules.EntityFrameworkCore.SqlServer;
//using Microsoft.EntityFrameworkCore;
//using Microsoft.Extensions.Configuration;
//using Microsoft.Extensions.DependencyInjection;

//namespace HnMicro.Modules.EntityFrameworkCore.Helpers
//{
//    public static class DbContextHelper
//    {
//        public static void AddDataContext<T>(this IServiceCollection services, IConfiguration configuration)
//            where T : DbContext
//        {
//            var sqlConnection = configuration.BindValue<SqlServerOption>(SqlServerOption.SectionName);
//            if (sqlConnection != null)
//            {
//                services.AddSqlServer<T>(sqlConnection);
//            }
//        }

//        public static void AddDataContext<T>(this IServiceCollection services, SqlServerOption sqlOption)
//            where T : DbContext
//        {
//            services.AddSqlServer<T>(sqlOption);
//        }
//    }
//}
