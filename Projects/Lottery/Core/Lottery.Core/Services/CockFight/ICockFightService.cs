using HnMicro.Core.Scopes;
using Lottery.Core.Dtos.CockFight;

namespace Lottery.Core.Services.CockFight
{
    public interface ICockFightService : IScopedDependency
    {
        Task CreateCockFightPlayer();
        Task LoginCockFightPlayer();
        Task<LoginPlayerInformationDto> GetCockFightUrl();
    }
}
