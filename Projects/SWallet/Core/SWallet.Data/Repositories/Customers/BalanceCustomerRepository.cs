using HnMicro.Modules.EntityFrameworkCore.Repositories;
using Microsoft.EntityFrameworkCore;
using SWallet.Data.Core;
using SWallet.Data.Core.Entities;

namespace SWallet.Data.Repositories.Customers
{
    public class BalanceCustomerRepository : EntityFrameworkCoreRepository<long, BalanceCustomer, SWalletContext>, IBalanceCustomerRepository
    {
        public BalanceCustomerRepository(SWalletContext context) : base(context)
        {
        }

        public async Task<BalanceCustomer> FindByCustomerId(long customerId)
        {
            return await DbSet.FirstOrDefaultAsync(f => f.CustomerId == customerId);
        }
    }
}
