namespace Lottery.Core.Models.Agent.UpdateAgent
{
    public class UpdateAgentModel
    {
        public long AgentId { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public int? State { get; set; }
        public string Permissions { get; set; }
        public decimal? Credit { get; set; }
        public decimal? MemberMaxCredit { get; set; }
    }
}
