using HnMicro.Modules.EntityFrameworkCore.Repositories;
using SWallet.Data.Core;
using SWallet.Data.Core.Entities;

namespace SWallet.Data.Repositories.Managers
{
    public class ManagerSessionRepository : EntityFrameworkCoreRepository<long, ManagerSession, SWalletContext>, IManagerSessionRepository
    {
        public ManagerSessionRepository(SWalletContext context) : base(context)
        {
        }
    }
}
