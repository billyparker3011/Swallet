using HnMicro.Modules.EntityFrameworkCore.Repositories;
using Lottery.Data;
using Lottery.Data.Entities.Partners.CockFight;

namespace Lottery.Core.Repositories.CockFight
{
    public class CockFightBetKindRepository : EntityFrameworkCoreRepository<int, CockFightBetKind, LotteryContext>, ICockFightBetKindRepository
    {
        public CockFightBetKindRepository(LotteryContext context) : base(context)
        {
        }
    }
}
