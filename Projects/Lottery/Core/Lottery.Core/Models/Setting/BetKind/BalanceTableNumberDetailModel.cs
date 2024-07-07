namespace Lottery.Core.Models.Setting.BetKind
{
    public class BalanceTableNumberDetailModel
    {
        public List<int> Numbers { get; set; } = new List<int>();
        public List<BalanceTableRateModel> RateValues { get; set; } = new List<BalanceTableRateModel>();
    }
}
