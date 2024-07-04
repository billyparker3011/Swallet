using HnMicro.Core.Scopes;

namespace Lottery.Core.Services.Setting
{
    public interface IBalanceTableSettingService : IScopedDependency
    {
        string CreateBalanceTableKey(int betKindId);
    }
}
