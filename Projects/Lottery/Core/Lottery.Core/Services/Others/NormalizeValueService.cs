namespace Lottery.Core.Services.Others
{
    public class NormalizeValueService : INormalizeValueService
    {
        public decimal Normalize(decimal value)
        {
            return Math.Round(value, 0, MidpointRounding.AwayFromZero);
        }

        public decimal GetRateValue(int betKindId, int number, Dictionary<int, Dictionary<int, decimal>> rateOfOddsValue)
        {
            if (!rateOfOddsValue.TryGetValue(betKindId, out Dictionary<int, decimal> dictRateValue)) return 0m;
            if (!dictRateValue.TryGetValue(number, out decimal rateValue)) return 0m;
            return rateValue;
        }
    }
}
