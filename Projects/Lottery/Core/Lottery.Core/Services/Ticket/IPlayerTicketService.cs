using HnMicro.Core.Scopes;
using Lottery.Core.Models.Ticket;
using Lottery.Core.Models.Winlose;

namespace Lottery.Core.Services.Ticket;

public interface IPlayerTicketService : IScopedDependency
{
    Task<List<TicketDetailModel>> GetDetailTicket(long ticketId, bool fromPlayer = true);
    Task<List<TicketDetailModel>> GetTicketsAsBetList();
    Task<List<TicketDetailModel>> GetTicketsByMatch(long matchId);

    Task<List<TicketDetailModel>> GetPlayerOuts(long playerId);
    Task<List<TicketDetailModel>> GetPlayerWinloseDetail(WinloseDetailQueryModel model);
}
