namespace Lottery.Agent.AgentService.Requests.Odds
{
    public class ChangeOddsValueOfOddsTableRequest
    {
        public int Number { get; set; }
        public bool Increment { get; set; }
        public decimal Rate { get; set; } = 1m;
    }
}
