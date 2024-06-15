using HnMicro.Core.Scopes;
using Lottery.Core.Models.Ticket;

namespace Lottery.Core.Services.Ticket;

public interface IAgentTicketService : IScopedDependency
{
    Task<AgentTicketResultModel> LatestTickets(AgentTicketModel model);
    Task<AgentTicketResultModel> GetRefundRejectTickets(AgentTicketModel model);
}
