using HnMicro.Modules.EntityFrameworkCore.SqlServer;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace SWallet.Data.Core
{
    public class SWalletContextFactory : IDesignTimeDbContextFactory<SWalletContext>
    {
        public SWalletContext CreateDbContext(string[] args)
        {
            var connectionString = DbContextHelper.GetConnectionString();
            var dbBuilder = new DbContextOptionsBuilder<SWalletContext>();
            dbBuilder.UseSqlServer(connectionString);
            return new SWalletContext(dbBuilder.Options);
        }
    }
}
