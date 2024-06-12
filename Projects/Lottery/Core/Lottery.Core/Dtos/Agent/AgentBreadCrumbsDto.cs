namespace Lottery.Core.Dtos.Agent
{
    public class AgentBreadCrumbsDto
    {
        public long? AgentId { get; set; }
        public int RoleId { get; set; }
        public string Username { get; set; }
        public bool IsActive { get; set; }
    }
}
