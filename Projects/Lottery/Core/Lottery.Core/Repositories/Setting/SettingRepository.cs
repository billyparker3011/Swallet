using HnMicro.Modules.EntityFrameworkCore.Repositories;
using Lottery.Data;

namespace Lottery.Core.Repositories.Setting
{
    public class SettingRepository : EntityFrameworkCoreRepository<int, Data.Entities.Setting, LotteryContext>, ISettingRepository
    {
        public SettingRepository(LotteryContext context) : base(context)
        {
        }
    }
}
