using HnMicro.Modules.EntityFrameworkCore.Repositories;
using Lottery.Data;

namespace Lottery.Core.Repositories.Bti
{
    public class BtiPlayerBetSettingRepository : EntityFrameworkCoreRepository<long, Data.Entities.Partners.Bti.BtiPlayerBetSetting, LotteryContext>, IBtiPlayerBetSettingRepository
    {
        public BtiPlayerBetSettingRepository(LotteryContext context) : base(context)
        {
        }
    }
}
