namespace Lottery.Match.MatchService.Requests.Match
{
    public class CreateOrUpdateMatchRequest
    {
        public DateTime KickOff { get; set; }
        public bool IncludeTime { get; set; }
        public bool AllowBeforeDate { get; set; }
    }
}
