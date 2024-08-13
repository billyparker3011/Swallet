using HnMicro.Modules.EntityFrameworkCore.Repositories;
using Lottery.Data;
using Lottery.Data.Entities.Partners.CockFight;

namespace Lottery.Core.Repositories.CockFight
{
    public interface ICockFightAgentPositionTakingRepository : IEntityFrameworkCoreRepository<long, CockFightAgentPostionTaking, LotteryContext>
    {

    }
}
