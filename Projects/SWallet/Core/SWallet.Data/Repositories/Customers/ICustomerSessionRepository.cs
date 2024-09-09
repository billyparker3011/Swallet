using HnMicro.Modules.EntityFrameworkCore.Repositories;
using SWallet.Data.Core;
using SWallet.Data.Core.Entities;

namespace SWallet.Data.Repositories.Customers
{
    public interface ICustomerSessionRepository : IEntityFrameworkCoreRepository<long, CustomerSession, SWalletContext>
    {
    }
}
