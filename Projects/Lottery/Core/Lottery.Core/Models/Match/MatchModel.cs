using Lottery.Core.Models.MatchResult;

namespace Lottery.Core.Models.Match;

public class MatchModel
{
    public long MatchId { get; set; }
    public string MatchCode { get; set; }
    public DateTime KickoffTime { get; set; }
    public int State { get; set; }
    public DateTime CreatedAt { get; set; }
    public Dictionary<int, List<ResultByRegionModel>> MatchResult { get; set; }
}
