using HnMicro.Modules.EntityFrameworkCore.Repositories;
using Lottery.Core.Repositories.Agent;
using Lottery.Data;

namespace Lottery.Core.Repositories.Partners.CA
{
    public class CAAgentPositionTakingRepository : EntityFrameworkCoreRepository<long, Data.Entities.Partners.CA.CAAgentPositionTaking, LotteryContext>, ICAAgentPositionTakingRepository
    {
        public CAAgentPositionTakingRepository(LotteryContext context) : base(context)
        {
        }

    }
}
