
using HnMicro.Core.Scopes;
using Lottery.Core.Models.Ticket;
using Lottery.Core.Partners.Models.Bti;


namespace Lottery.Core.Services.Partners.Bti
{
    public interface IBtiAgentTicketService : IScopedDependency
    {
          Task<BtiAgentTicketModel> GetBtiRefundRejectTickets(AgentTicketModel model);

          Task<BtiAgentTicketModel> BtiLatestTickets(AgentTicketModel model);
    }
}
