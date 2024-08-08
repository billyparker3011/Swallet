namespace Lottery.Agent.AgentService.Requests.CockFight
{
    public class UpdateCockFightAgentPositionTakingRequest
    {
        public long BetKindId { get; set; }
        public decimal DefaultPositionTaking { get; set; }
        public decimal ActualPositionTaking { get; set; }
    }
}
