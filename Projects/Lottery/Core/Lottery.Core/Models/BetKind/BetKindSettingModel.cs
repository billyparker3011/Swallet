namespace Lottery.Core.Models.BetKind;

public class BetKindSettingModel : BetKindModel
{
    public decimal MinBuy { get; set; }
    public decimal MaxBuy { get; set; }
    public decimal ActualBuy { get; set; }
}
