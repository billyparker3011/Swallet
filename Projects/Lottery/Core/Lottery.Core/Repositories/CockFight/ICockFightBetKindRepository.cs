using HnMicro.Modules.EntityFrameworkCore.Repositories;
using Lottery.Data;

namespace Lottery.Core.Repositories.CockFight
{
    public interface ICockFightBetKindRepository : IEntityFrameworkCoreRepository<long, Data.Entities.CockFightBetKind, LotteryContext>
    {

    }
}
