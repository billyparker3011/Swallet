using HnMicro.Core.Scopes;

namespace Lottery.Core.Services.Ticket;

public interface ICompletedMatchService : ISingletonDependency
{
    void Enqueue(long matchId);
}
