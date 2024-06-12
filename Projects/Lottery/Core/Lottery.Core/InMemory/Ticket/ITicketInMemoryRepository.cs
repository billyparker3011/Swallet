using HnMicro.Modules.InMemory.Repositories;
using Lottery.Core.Models.Ticket;

namespace Lottery.Core.InMemory.Ticket
{
    public interface ITicketInMemoryRepository : IInMemoryRepository<long, TicketModel>
    {
        List<TicketModel> GetTopSequenceTickets(double timeToAcceptOrRejectTicketInSeconds = 5d, int top = 100);
    }
}
