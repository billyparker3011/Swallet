using HnMicro.Modules.EntityFrameworkCore.Repositories;
using Microsoft.EntityFrameworkCore;
using SWallet.Data.Core;
using SWallet.Data.Core.Entities;

namespace SWallet.Data.Repositories.Managers
{
    public class ManagerRepository : EntityFrameworkCoreRepository<long, Manager, SWalletContext>, IManagerRepository
    {
        public ManagerRepository(SWalletContext context) : base(context)
        {
        }

        public async Task<Manager> FindByUsernameAndPassword(string username, string password)
        {
            return await DbSet.FirstOrDefaultAsync(f => f.Username.Equals(username) && f.Password.Equals(password));
        }

        public async Task<Manager> FindByUsername(string username)
        {
            return await DbSet.FirstOrDefaultAsync(f => f.Username.Equals(username));
        }

        public async Task<bool> AnyRoot(int managerRole, int roleId)
        {
            return await DbSet.AnyAsync(f => f.ManagerRole == managerRole && f.RoleId == roleId);
        }
    }
}
