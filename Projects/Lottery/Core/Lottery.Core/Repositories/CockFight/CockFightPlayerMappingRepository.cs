using HnMicro.Modules.EntityFrameworkCore.Repositories;
using Lottery.Data;

namespace Lottery.Core.Repositories.CockFight
{
    public class CockFightPlayerMappingRepository : EntityFrameworkCoreRepository<long, Data.Entities.CockFightPlayerMapping, LotteryContext>, ICockFightPlayerMappingRepository
    {
        public CockFightPlayerMappingRepository(LotteryContext context) : base(context)
        {
        }
    }
}
