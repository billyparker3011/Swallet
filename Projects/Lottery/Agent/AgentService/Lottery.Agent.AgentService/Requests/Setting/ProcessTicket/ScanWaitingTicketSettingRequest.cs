namespace Lottery.Agent.AgentService.Requests.Setting.ProcessTicket
{
    public class ScanWaitingTicketSettingRequest
    {
        public ScanWaitingTicketSettingDetailRequest NoneLive { get; set; } = new ScanWaitingTicketSettingDetailRequest();
        public ScanWaitingTicketSettingDetailRequest Live { get; set; } = new ScanWaitingTicketSettingDetailRequest();
    }
}
