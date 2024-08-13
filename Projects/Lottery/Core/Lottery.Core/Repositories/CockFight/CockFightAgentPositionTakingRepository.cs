using HnMicro.Modules.EntityFrameworkCore.Repositories;
using Lottery.Data;
using Lottery.Data.Entities.Partners.CockFight;

namespace Lottery.Core.Repositories.CockFight
{
    public class CockFightAgentPositionTakingRepository : EntityFrameworkCoreRepository<long, CockFightAgentPostionTaking, LotteryContext>, ICockFightAgentPositionTakingRepository
    {
        public CockFightAgentPositionTakingRepository(LotteryContext context) : base(context)
        {
        }
    }
}
