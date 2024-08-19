using HnMicro.Modules.EntityFrameworkCore.Repositories;
using Lottery.Data;
using Lottery.Data.Entities.Partners.CockFight;
using Microsoft.EntityFrameworkCore;

namespace Lottery.Core.Repositories.CockFight
{
    public class CockFightPlayerMappingRepository : EntityFrameworkCoreRepository<long, CockFightPlayerMapping, LotteryContext>, ICockFightPlayerMappingRepository
    {
        public CockFightPlayerMappingRepository(LotteryContext context) : base(context)
        {
        }

        public async Task<CockFightPlayerMapping> FindByMemberRefId(string memberRefId)
        {
            return await DbSet.FirstOrDefaultAsync(f => f.MemberRefId == memberRefId);
        }
    }
}
