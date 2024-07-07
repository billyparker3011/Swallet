using HnMicro.Core.Scopes;
using Lottery.Core.Models.Setting.BetKind;

namespace Lottery.Core.Services.Setting
{
    public interface IBalanceTableSettingService : IScopedDependency
    {
        string CreateBalanceTableKey(int betKindId);
        Task CreateBalanceTableSetting(int betKindId, BalanceTableModel model);
    }
}
