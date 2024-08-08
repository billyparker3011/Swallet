using HnMicro.Modules.EntityFrameworkCore.Repositories;
using Lottery.Data;

namespace Lottery.Core.Repositories.CockFight
{
    public interface ICockFightAgentPositionTakingRepository : IEntityFrameworkCoreRepository<long, Data.Entities.CockFightAgentPostionTaking, LotteryContext>
    {

    }
}
