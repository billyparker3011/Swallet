using HnMicro.Core.Scopes;
using Lottery.Core.Models.Ticket;
using Lottery.Core.Partners.Models.Allbet;

namespace Lottery.Core.Services.Partners.CA
{
    public interface ICasinoAgentTicketService : IScopedDependency
    {
        Task<CasinoAgentTicketModel> GetCasinoLatestTickets(AgentTicketModel model);
        Task<CasinoAgentTicketModel> GetCasinoRefundRejectTickets(AgentTicketModel model);
    }
}
