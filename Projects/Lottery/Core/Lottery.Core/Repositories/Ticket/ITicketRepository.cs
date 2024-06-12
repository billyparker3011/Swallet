using HnMicro.Modules.EntityFrameworkCore.Repositories;
using Lottery.Data;

namespace Lottery.Core.Repositories.Ticket;

public interface ITicketRepository : IEntityFrameworkCoreRepository<long, Data.Entities.Ticket, LotteryContext>
{
}
