namespace Lottery.Core.Models.Authentication
{
    public class SessionModel
    {
        public string Hash { get; set; }
        public string IpAddress { get; set; }
        public string UserAgent { get; set; }
        public string Platform { get; set; }
        public DateTime LatestDoingTime { get; set; }
        public int RoleId { get; set; }
        public long TargetId { get; set; }
        public DateTime LatestCheckingState { get; set; }
    }
}
