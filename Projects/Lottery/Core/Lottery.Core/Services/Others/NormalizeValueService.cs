namespace Lottery.Core.Services.Others
{
    public class NormalizeValueService : INormalizeValueService
    {
        public decimal Normalize(decimal value)
        {
            return Math.Round(value, 0, MidpointRounding.AwayFromZero);
        }
    }
}
