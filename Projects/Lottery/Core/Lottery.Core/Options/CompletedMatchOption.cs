namespace Lottery.Core.Options
{
    public class CompletedMatchOption
    {
        public const string AppSettingName = "CompletedMatch";

        public int IntervalInMilliseconds { get; set; }
        public int TopTickets { get; set; }
    }
}
