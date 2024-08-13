using HnMicro.Modules.EntityFrameworkCore.Repositories;
using Lottery.Data;

namespace Lottery.Core.Repositories.Casino
{
    public class CasinoAgentPositionTakingRepository : EntityFrameworkCoreRepository<long, Data.Entities.Partners.Casino.CasinoAgentPositionTaking, LotteryContext>, ICasinoAgentPositionTakingRepository
    {
        public CasinoAgentPositionTakingRepository(LotteryContext context) : base(context)
        {
        }
    }
}