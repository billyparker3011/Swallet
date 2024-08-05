using HnMicro.Modules.EntityFrameworkCore.Repositories;
using Lottery.Data;

namespace Lottery.Core.Repositories.CockFight
{
    public class CockFightAgentBetSettingRepository : EntityFrameworkCoreRepository<long, Data.Entities.CockFightAgentBetSetting, LotteryContext>, ICockFightAgentBetSettingRepository
    {
        public CockFightAgentBetSettingRepository(LotteryContext context) : base(context)
        {
        }
    }
}
