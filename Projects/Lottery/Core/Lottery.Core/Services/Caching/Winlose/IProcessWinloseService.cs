using HnMicro.Core.Scopes;

namespace Lottery.Core.Services.Caching.Winlose
{
    public interface IProcessWinloseService : ISingletonDependency
    {
        Task UpdateWinloseCache(Dictionary<string, Dictionary<string, decimal>> items);
    }
}
