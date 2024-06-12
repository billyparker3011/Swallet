using HnMicro.Modules.EntityFrameworkCore.Repositories;
using Lottery.Data;

namespace Lottery.Core.Repositories.BetKind
{
    public interface IBetKindRepository : IEntityFrameworkCoreRepository<int, Data.Entities.BetKind, LotteryContext>
    {
    }
}
