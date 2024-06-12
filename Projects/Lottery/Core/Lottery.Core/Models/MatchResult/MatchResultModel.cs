namespace Lottery.Core.Models.MatchResult
{
    public class MatchResultModel
    {
        public long MatchId { get; set; }
        public int RegionId { get; set; }
        public int ChannelId { get; set; }
        public bool IsLive { get; set; }
        public List<PrizeMatchResultModel> Results { get; set; }
    }
}
