namespace Lottery.Match.MatchService.Requests.MatchResult
{
    public class MatchResultRequest
    {
        public bool IsLive { get; set; }
        public List<PrizeMatchResultRequest> Results { get; set; }
    }
}
