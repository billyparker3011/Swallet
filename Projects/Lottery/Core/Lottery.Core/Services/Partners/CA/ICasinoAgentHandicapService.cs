using HnMicro.Core.Scopes;
using Lottery.Data.Entities.Partners.Casino;

namespace Lottery.Core.Services.Partners.CA
{
    public interface ICasinoAgentHandicapService : IScopedDependency
    {
        Task<IEnumerable<CasinoAgentHandicap>> GetAgentHandicapsAsync(string type);
        Task<IEnumerable<CasinoAgentHandicap>> GetAllAgentHandicapsAsync();
    }
}
