namespace Lottery.Agent.AgentService.Requests.Setting.ProcessTicket
{
    public class ValidationPrizeSettingRequest
    {
        public int RegionId { get; set; }
        public int BetKindId { get; set; }
        public int Prize { get; set; }
    }
}
