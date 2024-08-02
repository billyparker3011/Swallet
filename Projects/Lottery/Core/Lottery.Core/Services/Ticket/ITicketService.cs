using HnMicro.Core.Scopes;
using Lottery.Core.Models.Ticket.Process;

namespace Lottery.Core.Services.Ticket;

public interface ITicketService : IScopedDependency
{
    Task LoadTicketsByMatch(long matchId, int top = -1);
    Task Process(ProcessTicketModel model);
    Task ProcessMixed(ProcessMixedTicketModel model);
    Task ProcessV2(ProcessTicketV2Model model);
}
