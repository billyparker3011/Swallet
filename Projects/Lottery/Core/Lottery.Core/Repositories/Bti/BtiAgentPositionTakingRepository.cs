using HnMicro.Modules.EntityFrameworkCore.Repositories;
using Lottery.Data;

namespace Lottery.Core.Repositories.Bti
{
    internal class BtiAgentPositionTakingRepository : EntityFrameworkCoreRepository<long, Data.Entities.Partners.Bti.BtiAgentPositionTaking, LotteryContext>, IBtiAgentPositionTakingRepository
    {
        public BtiAgentPositionTakingRepository(LotteryContext context) : base(context)
        {
        }
    }
}
