using Lottery.Core.Dtos.Agent;

namespace Lottery.Agent.AgentService.Requests.Agent
{
    public class UpdateAgentPositionTakingRequest
    {
        public List<AgentPositionTakingDto> PositionTakings { get; set; }
    }
}
