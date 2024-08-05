using HnMicro.Modules.EntityFrameworkCore.Repositories;
using Lottery.Data;

namespace Lottery.Core.Repositories.CockFight
{
    public class CockFightBetKindRepository : EntityFrameworkCoreRepository<long, Data.Entities.CockFightBetKind, LotteryContext>, ICockFightBetKindRepository
    {
        public CockFightBetKindRepository(LotteryContext context) : base(context)
        {
        }
    }
}
