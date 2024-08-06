using HnMicro.Modules.EntityFrameworkCore.Repositories;
using Lottery.Data;

namespace Lottery.Core.Repositories.Partners.CA
{
    public class CAAgentHandicapRepository : EntityFrameworkCoreRepository<long, Data.Entities.Partners.CA.CAAgentHandicap, LotteryContext>, ICAAgentHandicapRepository
    {
        public CAAgentHandicapRepository(LotteryContext context) : base(context)
        {
        }

    }
}
