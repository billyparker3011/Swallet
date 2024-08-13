using HnMicro.Modules.EntityFrameworkCore.Repositories;
using Lottery.Data;

namespace Lottery.Core.Repositories.Casino
{
    public class CasinoGameTypeRepository : EntityFrameworkCoreRepository<int, Data.Entities.Partners.Casino.CasinoGameType, LotteryContext>, ICasinoGameTypeRepository
    {
        public CasinoGameTypeRepository(LotteryContext context) : base(context)
        {
        }
    }
}
