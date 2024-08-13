using HnMicro.Modules.EntityFrameworkCore.Repositories;
using Lottery.Data;

namespace Lottery.Core.Repositories.Casino
{
    public class CasinoPlayerBetSettingAgentHandicapRepository : EntityFrameworkCoreRepository<long, Data.Entities.Partners.Casino.CasinoPlayerBetSettingAgentHandicap, LotteryContext>, ICasinoPlayerBetSettingAgentHandicapRepository
    {
        public CasinoPlayerBetSettingAgentHandicapRepository(LotteryContext context) : base(context)
        {
        }
    }
}
