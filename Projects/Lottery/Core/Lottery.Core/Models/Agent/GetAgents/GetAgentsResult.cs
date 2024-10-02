using HnMicro.Framework.Responses;
using Lottery.Core.Dtos.Agent;

namespace Lottery.Core.Models.Agent.GetAgents
{
    public class GetAgentsResult
    {
        public IEnumerable<AgentDto> Agents { get; set; }
        public ApiResponseMetadata Metadata { get; set; }
    }
}
