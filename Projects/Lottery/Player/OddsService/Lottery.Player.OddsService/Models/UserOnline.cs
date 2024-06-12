namespace Lottery.Player.OddsService.Models
{
    public class UserOnline
    {
        public string ConnectionId { get; set; }
        public int RoleId { get; set; }
        public long PlayerId { get; set; }
        public long AgentId { get; set; }
        public long MasterId { get; set; }
        public long SupermasterId { get; set; }
        public DateTime ConnectionTime { get; set; }
        public int BetKindId { get; set; }
        public DateTime? PingTime { get; set; }
        public DateTime PongTime { get; set; }
    }
}
