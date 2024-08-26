using HnMicro.Core.Scopes;
using Lottery.Core.Partners.Models.Allbet;
using Lottery.Data.Entities.Partners.Casino;

namespace Lottery.Core.Services.Partners.CA
{
    public interface ICasinoAgentPositionTakingService : IScopedDependency
    {
        Task<CasinoAgentPositionTakingModel> FindAgentPositionTakingAsync(long id);

        Task<IEnumerable<CasinoAgentPositionTakingModel>> GetAgentPositionTakingsAsync(long agentId);

        Task<IEnumerable<CasinoAgentPositionTakingModel>> GetAllAgentPositionTakingsAsync();

        Task CreateAgentPositionTakingAsync(UpdateCasinoAgentPositionTakingModel model);

        Task UpdateAgentPositionTakingAsync(UpdateCasinoAgentPositionTakingModel model);

        Task DeleteAgentPositionTakingAsync(long id);

        Task<decimal> GetDefaultPositionTaking(long agentId, int betKindId);
    }
}
