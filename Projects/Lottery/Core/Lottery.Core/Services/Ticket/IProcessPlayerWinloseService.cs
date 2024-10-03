using HnMicro.Core.Scopes;
using HnMicro.Framework.Enums;

namespace Lottery.Core.Services.Ticket;

public interface IProcessPlayerWinloseService : ISingletonDependency
{
    void Enqueue(SportKind sportKind, Dictionary<long, Dictionary<string, decimal>> playerProcessed);
    Task Start();
}
