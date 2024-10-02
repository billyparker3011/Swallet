using HnMicro.Core.Scopes;
using Lottery.Core.Dtos.Agent;
using Lottery.Core.Models.Agent.GetAgentPositionTaking;
using Lottery.Core.Models.PositionTakings;

namespace Lottery.Core.Services.Agent
{
    public interface IAgentPositionTakingService : IScopedDependency
    {
        Task<GetAgentPositionTakingResult> GetAgentPositionTakings();
        Task<GetAgentPositionTakingResult> GetDetailAgentPositionTakings(long agentId);
        Task<Dictionary<int, List<AgentPositionTakingModel>>> GetAgentPositionTakingByAgentIds(List<int> betKindIds, List<long> agentIds);
        Task UpdateAgentPositionTakings(List<AgentPositionTakingDto> updateItems);
    }
}
