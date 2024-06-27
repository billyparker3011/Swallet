namespace Lottery.Core.Models.Setting.ProcessTicket
{
    public class ScanWaitingTicketSettingModel
    {
        public ScanWaitingTicketSettingDetailModel NoneLive { get; set; } = new ScanWaitingTicketSettingDetailModel();
        public ScanWaitingTicketSettingDetailModel Live { get; set; } = new ScanWaitingTicketSettingDetailModel();

        public static ScanWaitingTicketSettingModel CreateDefault()
        {
            return new ScanWaitingTicketSettingModel
            {
                NoneLive = new ScanWaitingTicketSettingDetailModel
                {
                    AllowAccepted = true,
                    IntervalAcceptedInSeconds = 5
                },
                Live = new ScanWaitingTicketSettingDetailModel
                {
                    AllowAccepted = true,
                    IntervalAcceptedInSeconds = 5
                }
            };
        }
    }
}
