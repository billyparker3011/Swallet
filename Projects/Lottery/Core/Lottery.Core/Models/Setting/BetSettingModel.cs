namespace Lottery.Core.Models.Setting
{
    public class BetSettingModel
    {
        public decimal MinBet { get; set; }
        public decimal MaxBet { get; set; }
        public decimal MaxPerNumber { get; set; }
        public decimal OddsValue { get; set; }
    }
}
