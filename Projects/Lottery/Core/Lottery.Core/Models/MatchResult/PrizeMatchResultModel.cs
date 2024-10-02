namespace Lottery.Core.Models.MatchResult
{
    public class PrizeMatchResultModel
    {
        public int Prize { get; set; }
        public string PrizeName { get; set; }
        public int Order { get; set; }
        public int NoOfNumbers { get; set; }
        public bool EnabledProcessTicket { get; set; }
        public List<PrizeMatchResultDetailModel> Results { get; set; } = new List<PrizeMatchResultDetailModel>();
    }
}
