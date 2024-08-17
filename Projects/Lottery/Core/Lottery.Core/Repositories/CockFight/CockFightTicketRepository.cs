using HnMicro.Modules.EntityFrameworkCore.Repositories;
using Lottery.Data;
using Lottery.Data.Entities.Partners.CockFight;

namespace Lottery.Core.Repositories.CockFight
{
    public class CockFightTicketRepository : EntityFrameworkCoreRepository<long, CockFightTicket, LotteryContext>, ICockFightTicketRepository
    {
        public CockFightTicketRepository(LotteryContext context) : base(context)
        {
        }
    }
}
