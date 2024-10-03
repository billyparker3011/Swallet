using HnMicro.Core.Scopes;

namespace Lottery.Core.Services.Ticket;

public interface IScanTicketService : ISingletonDependency
{
    void Start();
}
