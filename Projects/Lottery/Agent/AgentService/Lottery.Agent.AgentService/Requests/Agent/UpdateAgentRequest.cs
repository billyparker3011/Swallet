namespace Lottery.Agent.AgentService.Requests.Agent
{
    public class UpdateAgentRequest
    {
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public int? State { get; set; }
        public string Permissions { get; set; }
        public decimal? Credit { get; set; }
        public decimal? MemberMaxCredit { get; set; }
        public bool? IsLock { get; set; }
    }
}
