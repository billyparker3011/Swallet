using HnMicro.Modules.EntityFrameworkCore.Repositories;
using Lottery.Data;

namespace Lottery.Core.Repositories.Bti
{
    public class BtiPlayerMappingRepository : EntityFrameworkCoreRepository<long, Data.Entities.Partners.Bti.BtiPlayerMapping, LotteryContext>, IBtiPlayerMappingRepository
    {
        public BtiPlayerMappingRepository(LotteryContext context) : base(context)
        {
        }
    }
}
