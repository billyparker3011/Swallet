using Lottery.Core.Dtos.Agent;

namespace Lottery.Core.Models.Agent.GetAgentPositionTaking
{
    public class GetAgentPositionTakingResult
    {
        public IEnumerable<AgentPositionTakingDto> AgentPositionTakings { get; set; }
    }
}
