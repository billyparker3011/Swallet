namespace Lottery.Core.Models.Agent.GetAgentCreditBalance
{
    public class GetAgentCreditBalanceModel
    {
        public long? AgentId { get; set; }
        public string SearchTerm { get; set; }
        public int? State { get; set; }
    }
}
