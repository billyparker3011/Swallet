using HnMicro.Modules.EntityFrameworkCore.Repositories;
using Lottery.Data;

namespace Lottery.Core.Repositories.Casino
{
    public class CasinoAgentBetSettingRepository : EntityFrameworkCoreRepository<long, Data.Entities.Partners.Casino.CasinoAgentBetSetting, LotteryContext>, ICasinoAgentBetSettingRepository
    {
        public CasinoAgentBetSettingRepository(LotteryContext context) : base(context)
        {
        }
    }
}
