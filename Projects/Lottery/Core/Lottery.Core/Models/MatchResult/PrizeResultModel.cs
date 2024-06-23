namespace Lottery.Core.Models.MatchResult
{
    public class PrizeResultModel
    {
        public int Prize { get; set; }
        public string PrizeName { get; set; }
        public int Order { get; set; }
        public int NoOfNumbers { get; set; }
        public List<PrizeResultDetailModel> Results { get; set; } = new List<PrizeResultDetailModel>();
    }
}
