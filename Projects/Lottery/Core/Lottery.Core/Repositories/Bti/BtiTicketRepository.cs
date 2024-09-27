using HnMicro.Modules.EntityFrameworkCore.Repositories;
using Lottery.Data;

namespace Lottery.Core.Repositories.Bti
{
    public class BtiTicketRepository : EntityFrameworkCoreRepository<long, Data.Entities.Partners.Bti.BtiTicket, LotteryContext>, IBtiTicketRepository
    {
        public BtiTicketRepository(LotteryContext context) : base(context)
        {
        }
    }
}
