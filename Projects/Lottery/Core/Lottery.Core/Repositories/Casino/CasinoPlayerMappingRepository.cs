using HnMicro.Modules.EntityFrameworkCore.Repositories;
using Lottery.Data;

namespace Lottery.Core.Repositories.Casino
{
    public class CasinoPlayerMappingRepository : EntityFrameworkCoreRepository<long, Data.Entities.Partners.Casino.CasinoPlayerMapping, LotteryContext>, ICasinoPlayerMappingRepository
    {
        public CasinoPlayerMappingRepository(LotteryContext context) : base(context)
        {
        }
    }
}
