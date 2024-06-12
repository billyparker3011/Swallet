namespace Lottery.Core.Models.Setting
{
    public class PlayerBetSettingModel : BetSettingModel
    {
        public int RegionId { get; set; }
        public decimal Award { get; set; }
    }
}
