using HnMicro.Core.Scopes;

namespace Lottery.Core.Services.Agent
{
    public interface INormalizeDataService : IScopedDependency
    {
        Task DeleteSupermaster(long supermasterId);
        Task NormalizeAgents();
        Task NormalizePlayerBySupermaster(List<long> supermasterIds);
        Task NormalizePlayers();
    }
}
