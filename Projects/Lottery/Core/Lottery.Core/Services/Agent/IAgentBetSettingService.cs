using HnMicro.Core.Scopes;
using Lottery.Core.Dtos.Agent;
using Lottery.Core.Models.Agent.GetAgentBetSettings;

namespace Lottery.Core.Services.Agent
{
    public interface IAgentBetSettingService: IScopedDependency
    {
        Task<GetAgentBetSettingsResult> GetAgentBetSettings();
        Task<GetAgentBetSettingsResult> GetDetailAgentBetSettings(long agentId);
        Task UpdateAgentBetSettings(List<AgentBetSettingDto> updateItems);
    }
}
