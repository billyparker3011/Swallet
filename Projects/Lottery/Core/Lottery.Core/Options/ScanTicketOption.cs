namespace Lottery.Core.Options
{
    public class ScanTicketOption
    {
        public const string AppSettingName = "ScanTicket";

        public int IntervalInMilliseconds { get; set; }
    }
}
