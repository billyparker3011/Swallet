using HnMicro.Core.Scopes;
using Lottery.Core.Dtos.CockFight;
using Lottery.Core.Models.CockFight.GetCockFightPlayerWinlossDetail;

namespace Lottery.Core.Services.CockFight
{
    public interface ICockFightPlayerTicketService : IScopedDependency
    {
        Task<List<CockFightPlayerTicketDetailDto>> GetCockFightDetailTicket(long ticketId, bool fromPlayer = true);
        Task<List<CockFightPlayerTicketDetailDto>> GetCockFightTicketsAsBetList();
        Task<List<CockFightPlayerTicketDetailDto>> GetCockFightTicketsByMatch(long matchId);

        Task<List<CockFightPlayerTicketDetailDto>> GetCockFightPlayerOuts(long playerId);
        Task<List<CockFightPlayerTicketDetailDto>> GetCockFightPlayerWinloseDetail(GetCockFightPlayerWinlossDetailModel model);
        Task<List<CockFightPlayerTicketDetailDto>> GetCockFightRefundRejectTickets();
    }
}
