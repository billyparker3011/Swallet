namespace Lottery.Core.Models.Ticket
{
    public class BaseTicketDetailModel
    {
        public long TicketId { get; set; }
        public DateTime KickoffTime { get; set; }
        public int BetKindId { get; set; }
        public string BetKindName { get; set; }
        public int ChannelId { get; set; }
        public string ChannelName { get; set; }
        public int RegionId { get; set; }
        public string RegionName { get; set; }
        public int CategoryId { get; set; }
        public string CategoryName { get; set; }
        public string ChoosenNumbers { get; set; }
        public decimal? RewardRate { get; set; }
        public decimal? PlayerOdds { get; set; }
        public decimal TotalPoints { get; set; }
        public decimal TotalPayout { get; set; }
        public decimal? TotalWinlose { get; set; }
        public decimal? TotalDraftWinlose { get; set; }
        public int State { get; set; }
        public bool? ShowMore { get; set; }
        public string IpAddress { get; set; }
        public string Platform { get; set; }
        public bool IsLive { get; set; }
        public int? Prize { get; set; }
        public int? Position { get; set; }
        public int? Times { get; set; }
        public string MixedTimes { get; set; }
        public DateTime CreatedAt { get; set; }
    }
}
