using HnMicro.Modules.InMemory.Repositories;
using Lottery.Core.Models.Ticket;

namespace Lottery.Core.InMemory.Ticket
{
    public class TicketInMemoryRepository : InMemoryRepository<long, TicketModel>, ITicketInMemoryRepository
    {
        public List<TicketModel> GetTopSequenceTickets(double timeToAcceptOrRejectTicketInSeconds = 5d, int top = 100)
        {
            return Items.Values.Where(f => f.CreatedAt.AddSeconds(timeToAcceptOrRejectTicketInSeconds) <= DateTime.UtcNow).OrderBy(f => f.CreatedAt).Take(top).ToList();
        }

        protected override void InternalTryAddOrUpdate(TicketModel item)
        {
            Items[item.TicketId] = item;
        }

        protected override void InternalTryRemove(TicketModel item)
        {
            Items.TryRemove(item.TicketId, out _);
        }
    }
}
