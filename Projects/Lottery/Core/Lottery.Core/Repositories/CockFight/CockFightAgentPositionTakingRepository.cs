using HnMicro.Modules.EntityFrameworkCore.Repositories;
using Lottery.Data;

namespace Lottery.Core.Repositories.CockFight
{
    public class CockFightAgentPositionTakingRepository : EntityFrameworkCoreRepository<long, Data.Entities.CockFightAgentPostionTaking, LotteryContext>, ICockFightAgentPositionTakingRepository
    {
        public CockFightAgentPositionTakingRepository(LotteryContext context) : base(context)
        {
        }
    }
}
