using HnMicro.Modules.EntityFrameworkCore.Repositories;
using Microsoft.EntityFrameworkCore;
using SWallet.Data.Core;
using SWallet.Data.Core.Entities;

namespace SWallet.Data.Repositories.Customers
{
    public class LevelRepository : EntityFrameworkCoreRepository<int, Level, SWalletContext>, ILevelRepository
    {
        public LevelRepository(SWalletContext context) : base(context)
        {
        }

        public async Task<List<Level>> GetLevelByIds(List<int> levelIds)
        {
            return await DbSet.Where(f => levelIds.Contains(f.LevelId)).ToListAsync();
        }
    }
}
