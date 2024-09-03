using Lottery.Core.Models.Ticket;

namespace Lottery.Core.Partners.Models.Allbet
{
    public interface ICasinoAgentTicketService
    {
        Task<CasinoAgentTicketModel> GetCasinoLatestTickets(AgentTicketModel model);
        Task<CasinoAgentTicketModel> GetCasinoRefundRejectTickets(AgentTicketModel model);
    }
}
