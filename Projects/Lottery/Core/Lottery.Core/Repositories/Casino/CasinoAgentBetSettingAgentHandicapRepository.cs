using HnMicro.Modules.EntityFrameworkCore.Repositories;
using Lottery.Data;

namespace Lottery.Core.Repositories.Casino
{
    public class CasinoAgentBetSettingAgentHandicapRepository : EntityFrameworkCoreRepository<long, Data.Entities.Partners.Casino.CasinoAgentBetSettingAgentHandicap, LotteryContext>, ICasinoAgentBetSettingAgentHandicapRepository
    {
        public CasinoAgentBetSettingAgentHandicapRepository(LotteryContext context) : base(context)
        {
        }
    }
}
