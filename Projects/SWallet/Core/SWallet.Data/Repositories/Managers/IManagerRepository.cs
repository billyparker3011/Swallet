using HnMicro.Modules.EntityFrameworkCore.Repositories;
using SWallet.Data.Core;
using SWallet.Data.Core.Entities;

namespace SWallet.Data.Repositories.Managers
{
    public interface IManagerRepository : IEntityFrameworkCoreRepository<long, Manager, SWalletContext>
    {
        Task<bool> AnyRoot(int managerRole, int roleId);
        Task<Manager> FindByAffiliateCode(string affiliateCode);
        Task<Manager> FindByUsername(string username);
        Task<Manager> FindByUsernameAndPassword(string username, string password);
    }
}
