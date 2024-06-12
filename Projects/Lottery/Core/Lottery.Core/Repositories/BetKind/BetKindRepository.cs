using HnMicro.Modules.EntityFrameworkCore.Repositories;
using Lottery.Data;

namespace Lottery.Core.Repositories.BetKind
{
    public class BetKindRepository : EntityFrameworkCoreRepository<int, Data.Entities.BetKind, LotteryContext>, IBetKindRepository
    {
        public BetKindRepository(LotteryContext context) : base(context)
        {
        }
    }
}
