using HnMicro.Core.Scopes;

namespace Lottery.Core.Services.Agent
{
    public interface INormalizePlayerService : IScopedDependency
    {
        Task DeleteSupermaster(long supermasterId);
        Task NormalizePlayerBySupermaster(List<long> supermasterIds);
    }
}
