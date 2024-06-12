using HnMicro.Framework.Enums;

namespace Lottery.Core.Models.Agent.GetAgentOutstanding
{
    public class GetAgentOutstandingModel
    {
        public long? AgentId { get; set; }
        public int? RoleId { get; set; }
        public string SortName { get; set; }
        public SortType SortType { get; set; }  
    }
}
