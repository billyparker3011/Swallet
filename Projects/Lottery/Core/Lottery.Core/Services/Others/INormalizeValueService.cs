using HnMicro.Core.Scopes;

namespace Lottery.Core.Services.Others
{
    public interface INormalizeValueService : ISingletonDependency
    {
        decimal Normalize(decimal value);
        decimal GetRateValue(int betKindId, int number, Dictionary<int, Dictionary<int, decimal>> rateOfOddsValue);
    }
}
