using HnMicro.Modules.EntityFrameworkCore.Repositories;
using Microsoft.EntityFrameworkCore;
using SWallet.Data.Core;
using SWallet.Data.Core.Entities;

namespace SWallet.Data.Repositories.Customers
{
    public class CustomerBankAccountRepository : EntityFrameworkCoreRepository<long, CustomerBankAccount, SWalletContext>, ICustomerBankAccountRepository
    {
        public CustomerBankAccountRepository(SWalletContext context) : base(context)
        {
        }

        public async Task<CustomerBankAccount> FindByIdAndCustomer(long customerBankAccountId, long customerId)
        {
            return await DbSet.Include(f => f.Bank).FirstOrDefaultAsync(f => f.Id == customerBankAccountId && f.CustomerId == customerId);
        }
    }
}
