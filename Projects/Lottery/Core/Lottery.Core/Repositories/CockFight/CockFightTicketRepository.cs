using HnMicro.Modules.EntityFrameworkCore.Repositories;
using Lottery.Data;
using Lottery.Data.Entities.Partners.CockFight;
using Microsoft.EntityFrameworkCore;

namespace Lottery.Core.Repositories.CockFight
{
    public class CockFightTicketRepository : EntityFrameworkCoreRepository<long, CockFightTicket, LotteryContext>, ICockFightTicketRepository
    {
        public CockFightTicketRepository(LotteryContext context) : base(context)
        {
        }

        public async Task<CockFightTicket> FindBySId(string sid)
        {
            return await DbSet.FirstOrDefaultAsync(f => f.Sid == sid);
        }
    }
}
