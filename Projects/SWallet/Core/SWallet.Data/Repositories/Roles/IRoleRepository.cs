using HnMicro.Modules.EntityFrameworkCore.Repositories;
using SWallet.Data.Core;
using SWallet.Data.Core.Entities;

namespace SWallet.Data.Repositories.Roles
{
    public interface IRoleRepository : IEntityFrameworkCoreRepository<int, Role, SWalletContext>
    {
        Task<Role> GetRoleByRoleCode(string roleCode);
        Task<List<Role>> GetRoleByRoleCode(List<string> roleCode);
    }
}
