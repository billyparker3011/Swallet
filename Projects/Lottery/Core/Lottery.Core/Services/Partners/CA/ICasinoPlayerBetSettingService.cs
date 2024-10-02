using HnMicro.Core.Scopes;
using Lottery.Core.Partners.Models.Allbet;
using Lottery.Data.Entities.Partners.Casino;

namespace Lottery.Core.Services.Partners.CA
{
    public interface ICasinoPlayerBetSettingService : IScopedDependency
    {
        Task<CasinoPlayerBetSetting> FindPlayerBetSettingAsync(long id);

        Task<CasinoPlayerBetSetting> FindPlayerBetSettingWithIncludeAsync(long id);

        Task<IEnumerable<CasinoPlayerBetSetting>> GetPlayerBetSettingsAsync(long playerId);

        Task<IEnumerable<CasinoPlayerBetSetting>> GetPlayerBetSettingsWithIncludeAsync(long playerId);

        Task<IEnumerable<CasinoPlayerBetSetting>> GetAllPlayerBetSettingsAsync();

        Task CreatePlayerBetSettingAsync(CreateCasinoPlayerBetSettingModel model);

        Task UpdatePlayerBetSettingAsync(UpdateCasinoPlayerBetSettingModel model);

        Task DeletePlayerBetSettingAsync(long id);
        string[] GetStringGeneralHandicaps(CasinoPlayerBetSetting item);
    }
}
