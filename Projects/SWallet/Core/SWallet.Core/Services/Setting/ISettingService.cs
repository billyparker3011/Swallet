using HnMicro.Core.Scopes;
using SWallet.Core.Models.Settings;

namespace SWallet.Core.Services.Setting
{
    public interface ISettingService : IScopedDependency
    {
        Task<SettingModel> GetSetting();
        Task UpdateSetting(SettingModel model);
    }
}
