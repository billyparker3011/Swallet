using HnMicro.Modules.LoggerService.SqlProvider.Helpers;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace HnMicro.Modules.LoggerService.SqlProvider.Data
{
    public class LoggerContextFactory : IDesignTimeDbContextFactory<LoggerContext>
    {
        public LoggerContext CreateDbContext(string[] args)
        {
            var connectionString = LoggerSqlProviderHelper.GetConnectionStringForLoggerSqlProvider();
            var dbBuilder = new DbContextOptionsBuilder<LoggerContext>();
            dbBuilder.UseSqlServer(connectionString);
            return new LoggerContext(dbBuilder.Options);
        }
    }
}
