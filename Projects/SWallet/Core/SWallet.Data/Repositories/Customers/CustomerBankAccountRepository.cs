using HnMicro.Modules.EntityFrameworkCore.Repositories;
using SWallet.Data.Core;
using SWallet.Data.Core.Entities;

namespace SWallet.Data.Repositories.Customers
{
    public class CustomerBankAccountRepository : EntityFrameworkCoreRepository<long, CustomerBankAccount, SWalletContext>, ICustomerBankAccountRepository
    {
        public CustomerBankAccountRepository(SWalletContext context) : base(context)
        {
        }
    }
}
