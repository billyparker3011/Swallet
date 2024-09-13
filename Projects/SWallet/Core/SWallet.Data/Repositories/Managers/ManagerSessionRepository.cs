using HnMicro.Modules.EntityFrameworkCore.Repositories;
using Microsoft.EntityFrameworkCore;
using SWallet.Data.Core;
using SWallet.Data.Core.Entities;

namespace SWallet.Data.Repositories.Managers
{
    public class ManagerSessionRepository : EntityFrameworkCoreRepository<long, ManagerSession, SWalletContext>, IManagerSessionRepository
    {
        public ManagerSessionRepository(SWalletContext context) : base(context)
        {
        }

        public async Task<ManagerSession> FindByManagerId(long managerId)
        {
            return await DbSet.FirstOrDefaultAsync(f => f.ManagerId == managerId);
        }
    }
}
