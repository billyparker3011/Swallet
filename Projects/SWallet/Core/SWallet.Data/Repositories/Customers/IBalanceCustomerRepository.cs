using HnMicro.Modules.EntityFrameworkCore.Repositories;
using SWallet.Data.Core;
using SWallet.Data.Core.Entities;

namespace SWallet.Data.Repositories.Customers
{
    public interface IBalanceCustomerRepository : IEntityFrameworkCoreRepository<long, BalanceCustomer, SWalletContext>
    {
        Task<BalanceCustomer> FindByCustomerId(long customerId);
    }
}
