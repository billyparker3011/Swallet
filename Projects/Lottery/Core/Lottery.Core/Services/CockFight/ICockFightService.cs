using HnMicro.Core.Scopes;

namespace Lottery.Core.Services.Audit
{
    public interface ICockFightService : IScopedDependency
    {
        Task CreateCockFightPlayer();
        Task LoginCockFightPlayer();
    }
}
