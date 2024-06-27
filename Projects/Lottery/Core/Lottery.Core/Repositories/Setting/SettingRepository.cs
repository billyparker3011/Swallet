using HnMicro.Modules.EntityFrameworkCore.Repositories;
using Lottery.Data;
using Microsoft.EntityFrameworkCore;

namespace Lottery.Core.Repositories.Setting
{
    public class SettingRepository : EntityFrameworkCoreRepository<int, Data.Entities.Setting, LotteryContext>, ISettingRepository
    {
        public SettingRepository(LotteryContext context) : base(context)
        {
        }

        public async Task<Data.Entities.Setting> FindByKey(string key)
        {
            return await DbSet.FirstOrDefaultAsync(f => f.KeySetting == key);
        }
    }
}
