using HnMicro.Modules.EntityFrameworkCore.Repositories;
using Lottery.Data;

namespace Lottery.Core.Repositories.Bti
{
    public interface IBtiAgentPositionTakingRepository : IEntityFrameworkCoreRepository<long, Data.Entities.Partners.Bti.BtiAgentPositionTaking, LotteryContext>
    {

    }
}
