using HnMicro.Modules.EntityFrameworkCore.Repositories;
using Lottery.Data;

namespace Lottery.Core.Repositories.Prize
{
    public interface IPrizeRepository : IEntityFrameworkCoreRepository<int, Data.Entities.Prize, LotteryContext>
    {
    }
}
