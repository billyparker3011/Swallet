using HnMicro.Modules.InMemory.Repositories;
using Lottery.Core.Models.Ticket;

namespace Lottery.Core.InMemory.Ticket
{
    public interface ITicketInMemoryRepository : IInMemoryRepository<long, TicketModel>
    {
        List<TicketModel> GetTopSequenceTickets(bool isLive, double timeToAcceptOrRejectTicketInSeconds = 5d, int top = 100);
    }
}
