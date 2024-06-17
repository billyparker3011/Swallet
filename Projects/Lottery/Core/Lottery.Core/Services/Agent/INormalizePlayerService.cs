using HnMicro.Core.Scopes;

namespace Lottery.Core.Services.Agent
{
    public interface INormalizePlayerService : IScopedDependency
    {
        Task NormalizePlayerBySupermaster(List<long> supermasterIds);
    }
}
