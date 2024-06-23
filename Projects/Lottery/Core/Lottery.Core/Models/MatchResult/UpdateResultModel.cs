namespace Lottery.Core.Models.MatchResult
{
    public class UpdateResultModel
    {
        public long MatchId { get; set; }
        public int RegionId { get; set; }
        public int ChannelId { get; set; }
        public List<PrizeResultModel> Results { get; set; }
    }
}
