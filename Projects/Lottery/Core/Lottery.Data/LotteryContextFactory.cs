using HnMicro.Modules.EntityFrameworkCore.SqlServer;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace Lottery.Data
{
    public class LotteryContextFactory : IDesignTimeDbContextFactory<LotteryContext>
    {
        public LotteryContext CreateDbContext(string[] args)
        {
            var connectionString = DbContextHelper.GetConnectionString();
            var dbBuilder = new DbContextOptionsBuilder<LotteryContext>();
            dbBuilder.UseSqlServer(connectionString);
            return new LotteryContext(dbBuilder.Options);
        }
    }
}
