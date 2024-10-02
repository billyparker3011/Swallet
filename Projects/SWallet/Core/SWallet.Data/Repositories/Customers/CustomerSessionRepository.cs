using HnMicro.Modules.EntityFrameworkCore.Repositories;
using Microsoft.EntityFrameworkCore;
using SWallet.Data.Core;
using SWallet.Data.Core.Entities;

namespace SWallet.Data.Repositories.Customers
{
    public class CustomerSessionRepository : EntityFrameworkCoreRepository<long, CustomerSession, SWalletContext>, ICustomerSessionRepository
    {
        public CustomerSessionRepository(SWalletContext context) : base(context)
        {
        }

        public async Task<CustomerSession> FindByCustomerId(long customerId)
        {
            return await DbSet.FirstOrDefaultAsync(f => f.CustomerId == customerId);
        }
    }
}
