namespace Lottery.Core.Models.CockFight.GetCockFightPlayerWinlossDetail
{
    public class GetCockFightPlayerWinlossDetailModel
    {
        public long PlayerId { get; set; }
        public DateTime FromDate { get; set; }
        public DateTime ToDate { get; set; }
    }
}
