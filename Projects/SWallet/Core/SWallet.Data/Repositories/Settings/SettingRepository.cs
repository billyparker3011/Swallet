using HnMicro.Modules.EntityFrameworkCore.Repositories;
using Microsoft.EntityFrameworkCore;
using SWallet.Data.Core;
using SWallet.Data.Core.Entities;

namespace SWallet.Data.Repositories.Settings
{
    public class SettingRepository : EntityFrameworkCoreRepository<int, Setting, SWalletContext>, ISettingRepository
    {
        public SettingRepository(SWalletContext context) : base(context)
        {
        }

        public async Task<Setting> GetActualSetting()
        {
            return await DbSet.FirstOrDefaultAsync();
        }
    }
}
