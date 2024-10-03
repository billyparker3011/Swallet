using Lottery.Core.Dtos.Agent;

namespace Lottery.Agent.AgentService.Requests.Agent
{
    public class UpdateAgentBetSettingRequest
    {
        public List<AgentBetSettingDto> BetSettings { get; set; }
    }
}
