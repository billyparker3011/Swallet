namespace Lottery.Core.Models.MatchResult
{
    public class ResultByRegionModel
    {
        public int ChannelId { get; set; }
        public string ChannelName { get; set; }
        public bool IsLive { get; set; }
        public bool EnabledProcessTicket { get; set; }
        public int NoOfRemainingNumbers { get; set; }
        public List<PrizeResultModel> Prize { get; set; }
    }
}
