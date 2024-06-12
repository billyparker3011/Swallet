using HnMicro.Framework.Models;

namespace Lottery.Core.Models.Agent.GetAgents
{
    public class GetAgentsModel: QueryAdvance
    {
        public long? AgentId { get; set; }
        public string SearchTerm { get; set; }
        public int? State { get; set; }
    }
}
