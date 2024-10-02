using HnMicro.Modules.EntityFrameworkCore.Repositories;
using SWallet.Data.Core;
using SWallet.Data.Core.Entities;

namespace SWallet.Data.Repositories.Managers
{
    public interface IManagerSessionRepository : IEntityFrameworkCoreRepository<long, ManagerSession, SWalletContext>
    {
        Task<ManagerSession> FindByManagerId(long managerId);
    }
}
