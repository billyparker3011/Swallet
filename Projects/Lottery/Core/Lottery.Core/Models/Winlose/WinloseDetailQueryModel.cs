namespace Lottery.Core.Models.Winlose
{
    public class WinloseDetailQueryModel
    {
        public long TargetId { get; set; }
        public DateTime FromDate { get; set; }
        public DateTime ToDate { get; set; }
        public bool SelectedDraft { get; set; }
    }
}
