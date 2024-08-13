using HnMicro.Modules.EntityFrameworkCore.Repositories;
using Lottery.Data;

namespace Lottery.Core.Repositories.Casino
{
    public class CasinoBetKindRepository : EntityFrameworkCoreRepository<int, Data.Entities.Partners.Casino.CasinoBetKind, LotteryContext>, ICasinoBetKindRepository
    {
        public CasinoBetKindRepository(LotteryContext context) : base(context)
        {
        }
    }
}
