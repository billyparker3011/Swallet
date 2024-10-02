namespace Lottery.Core.Models.Client
{
    public class ClientPlayerModel : ClientModel
    {
        public long PlayerId { get; set; }
        public long AgentId { get; set; }
        public long MasterId { get; set; }
        public long SupermasterId { get; set; }
    }
}
