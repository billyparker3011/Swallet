using HnMicro.Modules.EntityFrameworkCore.Repositories;
using Lottery.Data;

namespace Lottery.Core.Repositories.CockFight
{
    public interface ICockFightPlayerMappingRepository : IEntityFrameworkCoreRepository<long, Data.Entities.CockFightPlayerMapping, LotteryContext>
    {

    }
}
