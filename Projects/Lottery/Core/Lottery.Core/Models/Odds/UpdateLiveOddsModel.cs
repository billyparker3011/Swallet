namespace Lottery.Core.Models.Odds
{
    public class UpdateLiveOddsModel
    {
        public long MatchId { get; set; }
        public int RegionId { get; set; }
        public int ChannelId { get; set; }
    }
}
