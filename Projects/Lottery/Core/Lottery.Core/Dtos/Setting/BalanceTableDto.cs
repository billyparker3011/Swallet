using Lottery.Core.Models.Setting.BetKind;

namespace Lottery.Core.Dtos.Setting
{
    public class BalanceTableDto
    {
        public BalanceTableCommonDetailModel ForCommon { get; set; }
        public BalanceTableNumberDetailDto ByNumbers { get; set; }
    }

    public class BalanceTableNumberDetailDto
    {
        public string Numbers { get; set; }
        public List<BalanceTableRateModel> RateValues { get; set; } = new List<BalanceTableRateModel>();
    }
}
