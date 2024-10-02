namespace Lottery.Core.Models.Client
{
    public class ClientAgentModel : ClientModel
    {
        public long AgentId { get; set; }
        public long ParentId { get; set; }
        public List<string> Permissions { get; set; }
        public long SupermasterId { get; set; }
        public long MasterId { get; set; }
    }
}
