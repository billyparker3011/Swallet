using HnMicro.Modules.EntityFrameworkCore.Repositories;
using Lottery.Data;

namespace Lottery.Core.Repositories.Prize
{
    public class PrizeRepository : EntityFrameworkCoreRepository<int, Data.Entities.Prize, LotteryContext>, IPrizeRepository
    {
        public PrizeRepository(LotteryContext context) : base(context)
        {
        }
    }
}
