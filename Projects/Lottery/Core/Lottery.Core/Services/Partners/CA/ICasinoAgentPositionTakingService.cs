using HnMicro.Core.Scopes;
using Lottery.Core.Partners.Models.Allbet;
using Lottery.Data.Entities.Partners.Casino;

namespace Lottery.Core.Services.Partners.CA
{
    public interface ICasinoAgentPositionTakingService : IScopedDependency
    {
        Task<CasinoAgentPositionTaking> FindAgentPositionTakingAsync(long id);

        Task<IEnumerable<CasinoAgentPositionTaking>> GetAgentPositionTakingsAsync(long agentId);

        Task<IEnumerable<CasinoAgentPositionTaking>> GetAllAgentPositionTakingsAsync();

        Task CreateAgentPositionTakingAsync(CreateCasinoAgentPositionTakingModel model);

        Task UpdateAgentPositionTakingAsync(UpdateCasinoAgentPositionTakingModel model);

        Task DeleteAgentPositionTakingAsync(long id);
    }
}
