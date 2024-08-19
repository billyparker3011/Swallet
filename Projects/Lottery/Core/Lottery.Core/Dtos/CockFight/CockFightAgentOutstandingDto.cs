using Lottery.Core.Enums;

namespace Lottery.Core.Dtos.CockFight
{
    public class CockFightAgentOutstandingDto
    {
        public long AgentId { get; set; }
        public string Username { get; set; }
        public Role AgentRole { get; set; }
        public long TotalBetCount { get; set; }
        public decimal TotalPayout { get; set; }
    }
}
