using HnMicro.Modules.EntityFrameworkCore.Repositories;
using SWallet.Data.Core;
using SWallet.Data.Core.Entities;

namespace SWallet.Data.Repositories.Customers
{
    public class CustomerSessionRepository : EntityFrameworkCoreRepository<long, CustomerSession, SWalletContext>, ICustomerSessionRepository
    {
        public CustomerSessionRepository(SWalletContext context) : base(context)
        {
        }
    }
}
