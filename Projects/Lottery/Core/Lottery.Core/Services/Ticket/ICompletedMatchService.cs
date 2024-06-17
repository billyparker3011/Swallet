using HnMicro.Core.Scopes;
using Lottery.Core.Models.Ticket;

namespace Lottery.Core.Services.Ticket;

public interface ICompletedMatchService : ISingletonDependency
{
    void Enqueue(CompletedMatchInQueueModel model);
}
