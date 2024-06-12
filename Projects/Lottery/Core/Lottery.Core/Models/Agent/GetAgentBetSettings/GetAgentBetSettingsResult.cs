using Lottery.Core.Dtos.Agent;

namespace Lottery.Core.Models.Agent.GetAgentBetSettings
{
    public class GetAgentBetSettingsResult
    {
        public IEnumerable<AgentBetSettingDto> AgentBetSettings { get; set; }
    }
}
