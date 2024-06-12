using Lottery.Core.Dtos.Agent;

namespace Lottery.Core.Models.Agent.GetSubAgents
{
    public class GetSubAgentsResult
    {
        public IEnumerable<SubAgentDto> SubAgents { get; set; }
    }
}
