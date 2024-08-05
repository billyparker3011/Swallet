using HnMicro.Core.Scopes;

namespace Lottery.Core.Services.CockFight
{
    public interface ICockFightService : IScopedDependency
    {
        Task CreateCockFightPlayer();
        Task LoginCockFightPlayer();
    }
}
