namespace Lottery.Core.Dtos.Agent
{
    public class AgentDto
    {
        public long Id { get; set; }
        public long? ParentId { get; set; }
        public string Username { get; set; }
        public int RoleId { get; set; }
        public int State { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public decimal Credit { get; set; }
        public decimal? MemberMaxCredit { get; set; }
        public DateTime CreatedDate { get; set; }
        public string IpAddress { get; set; }
        public string Platform { get; set; }
    }
}
