namespace Lottery.Agent.AgentService.Requests.Setting.ProcessTicket
{
    public class ScanWaitingTicketSettingDetailRequest
    {
        public bool AllowAccepted { get; set; }
        public int IntervalAcceptedInSeconds { get; set; }
    }
}
