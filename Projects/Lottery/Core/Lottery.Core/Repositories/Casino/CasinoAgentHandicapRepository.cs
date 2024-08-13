using HnMicro.Modules.EntityFrameworkCore.Repositories;
using Lottery.Data;

namespace Lottery.Core.Repositories.Casino
{
    public class CasinoAgentHandicapRepository : EntityFrameworkCoreRepository<int, Data.Entities.Partners.Casino.CasinoAgentHandicap, LotteryContext>, ICasinoAgentHandicapRepository
    {
        public CasinoAgentHandicapRepository(LotteryContext context) : base(context)
        {
        }
    }
}
