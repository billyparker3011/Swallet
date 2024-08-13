using HnMicro.Modules.EntityFrameworkCore.Repositories;
using Lottery.Data;
using Lottery.Data.Entities.Partners.CockFight;

namespace Lottery.Core.Repositories.CockFight
{
    public class CockFightPlayerMappingRepository : EntityFrameworkCoreRepository<long, CockFightPlayerMapping, LotteryContext>, ICockFightPlayerMappingRepository
    {
        public CockFightPlayerMappingRepository(LotteryContext context) : base(context)
        {
        }
    }
}
