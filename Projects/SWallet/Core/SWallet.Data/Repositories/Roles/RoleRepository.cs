using HnMicro.Modules.EntityFrameworkCore.Repositories;
using Microsoft.EntityFrameworkCore;
using SWallet.Data.Core;
using SWallet.Data.Core.Entities;

namespace SWallet.Data.Repositories.Roles
{
    public class RoleRepository : EntityFrameworkCoreRepository<int, Role, SWalletContext>, IRoleRepository
    {
        public RoleRepository(SWalletContext context) : base(context)
        {
        }

        public async Task<List<Role>> GetRoleByRoleCode(List<string> roleCode)
        {
            return await DbSet.Where(f => roleCode.Contains(f.RoleCode)).ToListAsync();
        }
    }
}
