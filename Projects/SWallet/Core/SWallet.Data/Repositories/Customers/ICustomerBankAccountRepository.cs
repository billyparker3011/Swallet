using HnMicro.Modules.EntityFrameworkCore.Repositories;
using SWallet.Data.Core;
using SWallet.Data.Core.Entities;

namespace SWallet.Data.Repositories.Customers
{
    public interface ICustomerBankAccountRepository : IEntityFrameworkCoreRepository<long, CustomerBankAccount, SWalletContext>
    {
        Task<CustomerBankAccount> FindByIdAndCustomer(long customerBankAccountId, long customerId);
    }
}
