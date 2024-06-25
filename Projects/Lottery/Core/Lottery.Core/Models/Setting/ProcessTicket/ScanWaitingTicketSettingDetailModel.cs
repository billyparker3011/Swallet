namespace Lottery.Core.Models.Setting.ProcessTicket
{
    public class ScanWaitingTicketSettingDetailModel
    {
        public bool AllowAccepted { get; internal set; }
        public int IntervalAcceptedInSeconds { get; internal set; }
    }
}
