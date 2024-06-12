using Lottery.Core.Enums;

namespace Lottery.Core.Dtos.Agent
{
    public class AgentOutstandingDto
    {
        public long AgentId { get; set;}
        public string Username { get; set; }
        public Role AgentRole { get; set; }
        public decimal TotalStake { get; set; }
        public decimal TotalPayout { get; set; }
        public string IpAddress { get; set; }
        public string Platform { get; set; }
    }
}
