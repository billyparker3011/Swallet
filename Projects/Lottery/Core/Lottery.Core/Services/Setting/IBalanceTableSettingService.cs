using HnMicro.Core.Scopes;
using Lottery.Core.Dtos.Setting;
using Lottery.Core.Models.Setting.BetKind;

namespace Lottery.Core.Services.Setting
{
    public interface IBalanceTableSettingService : IScopedDependency
    {
        string CreateBalanceTableKey(int betKindId);
        Task CreateOrModifyBetKindBalanceTableSetting(int betKindId, BalanceTableModel model);
        Task<BalanceTableDto> GetBetKindBalanceTableSetting(int betKindId);
    }
}
