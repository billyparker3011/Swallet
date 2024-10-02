namespace Lottery.Core.Models.Match.CreateMatch
{
    public class CreateOrUpdateMatchModel
    {
        public long MatchId { get; set; }
        public DateTime KickOff { get; set; }
        public bool IncludeTime { get; set; }
        public bool AllowBeforeDate { get; set; }
    }
}
