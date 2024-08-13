using HnMicro.Modules.EntityFrameworkCore.Repositories;
using Lottery.Data;

namespace Lottery.Core.Repositories.Casino
{
    public class CasinoPlayerBetSettingRepository : EntityFrameworkCoreRepository<long, Data.Entities.Partners.Casino.CasinoPlayerBetSetting, LotteryContext>, ICasinoPlayerBetSettingRepository
    {
        public CasinoPlayerBetSettingRepository(LotteryContext context) : base(context)
        {
        }
    }
}
