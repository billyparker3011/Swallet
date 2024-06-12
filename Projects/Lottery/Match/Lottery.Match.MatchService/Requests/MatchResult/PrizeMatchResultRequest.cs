namespace Lottery.Match.MatchService.Requests.MatchResult
{
    public class PrizeMatchResultRequest
    {
        public int Prize { get; set; }
        public int Order { get; set; }
        public bool EnabledProcessTicket { get; set; } = true;
        public List<PrizeMatchResultDetailRequest> Results { get; set; }
    }
}
