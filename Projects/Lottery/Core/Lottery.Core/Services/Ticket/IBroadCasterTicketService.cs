using HnMicro.Core.Scopes;
using Lottery.Core.Models.Ticket;
using Lottery.Core.Models.Winlose;

namespace Lottery.Core.Services.Ticket;

public interface IBroadCasterTicketService : IScopedDependency
{
    Task<List<TicketDetailModel>> GetBroadCasterOuts(int betkindId);
    Task<List<TicketDetailModel>> GetBroadCasterWinloseDetail(WinloseDetailQueryModel model);
}
