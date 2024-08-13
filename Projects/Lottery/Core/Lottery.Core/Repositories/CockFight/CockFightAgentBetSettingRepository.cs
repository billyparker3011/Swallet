using HnMicro.Modules.EntityFrameworkCore.Repositories;
using Lottery.Data;
using Lottery.Data.Entities.Partners.CockFight;

namespace Lottery.Core.Repositories.CockFight
{
    public class CockFightAgentBetSettingRepository : EntityFrameworkCoreRepository<long, CockFightAgentBetSetting, LotteryContext>, ICockFightAgentBetSettingRepository
    {
        public CockFightAgentBetSettingRepository(LotteryContext context) : base(context)
        {
        }
    }
}
