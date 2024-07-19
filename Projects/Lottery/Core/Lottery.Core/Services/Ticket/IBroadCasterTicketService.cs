using HnMicro.Core.Scopes;
using Lottery.Core.Models.Ticket;

namespace Lottery.Core.Services.Ticket;

public interface IBroadCasterTicketService : IScopedDependency
{
    Task<List<TicketDetailModel>> GetBroadCasterOuts(long betkindId);
}
