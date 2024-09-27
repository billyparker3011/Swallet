namespace Lottery.Core.Dtos.Agent
{
    public class SubAgentDto
    {
        public long Id { get; set; }
        public string Username { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public int State { get; set; }
        public int Role { get; set; }
        public bool IsLock { get; set; }
        public DateTime CreatedDate { get; set; }
        public string IpAddress { get; set; }
        public string Platform { get; set; }
        public List<string> Permissions { get; set; }
    }
}
