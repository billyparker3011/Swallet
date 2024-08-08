namespace Lottery.Core.Models.CockFight.GetCockFightAgentPositionTaking
{
    public class GetCockFightAgentPositionTakingResult
    {
        public long BetKindId { get; set; }
        public string BetKindName { get; set; }
        public decimal DefaultPositionTaking { get; set; }
        public decimal ActualPositionTaking { get; set; }
    }
}
