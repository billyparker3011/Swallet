namespace Lottery.Core.Models.MatchResult
{
    public class ResultModel
    {
        public long MatchId { get; set; }
        public DateTime KickoffTime { get; set; }
        public int State { get; set; }
        public Dictionary<int, List<ResultByRegionModel>> ResultByRegion { get; set; }
    }
}
