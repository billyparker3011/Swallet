using HnMicro.Core.Scopes;
using Lottery.Core.Models.CockFight.GetCockFightAgentTicket;
using Lottery.Core.Models.Ticket;

namespace Lottery.Core.Services.CockFight
{
    public interface ICockFightAgentTicketService : IScopedDependency
    {
        Task<CockFightAgentTicketResult> CockFightLatestTickets(AgentTicketModel model);
        Task<CockFightAgentTicketResult> GetCockFightRefundRejectTickets(AgentTicketModel model);
    }
}
