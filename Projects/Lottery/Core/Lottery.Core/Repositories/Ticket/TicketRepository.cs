using HnMicro.Modules.EntityFrameworkCore.Repositories;
using Lottery.Data;

namespace Lottery.Core.Repositories.Ticket;

public class TicketRepository : EntityFrameworkCoreRepository<long, Data.Entities.Ticket, LotteryContext>, ITicketRepository
{
    public TicketRepository(LotteryContext context) : base(context)
    {
    }
}