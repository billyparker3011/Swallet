using HnMicro.Modules.EntityFrameworkCore.Repositories;
using Lottery.Core.Repositories.Agent;
using Lottery.Data;

namespace Lottery.Core.Repositories.Partners.CA
{
    public interface ICAAgentPositionTakingRepository : IEntityFrameworkCoreRepository<long, Data.Entities.Partners.CA.CAAgentPositionTaking, LotteryContext>
    {

    }
}
