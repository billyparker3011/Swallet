using HnMicro.Core.Scopes;
using Lottery.Core.Partners.Models.Allbet;
using Lottery.Data.Entities.Partners.Casino;


namespace Lottery.Core.Services.Partners.CA
{
    public interface ICasinoAgentBetSettingService : IScopedDependency
    {
        Task<CasinoAgentBetSetting> FindAgentBetSettingAsync(long id);

        Task<CasinoAgentBetSetting> FindAgentBetSettingWithIncludeAsync(long id);

        Task<IEnumerable<CasinoAgentBetSetting>> GetAgentBetSettingsAsync(long agentId);

        Task<IEnumerable<CasinoAgentBetSetting>> GetAgentBetSettingsWithIncludeAsync(long agentId);

        Task<IEnumerable<CasinoAgentBetSetting>> GetAllAgentBetSettingsAsync();

        Task CreateAgentBetSettingAsync(CreateCasinoAgentBetSettingModel model);

        Task UpdateAgentBetSettingAsync(UpdateCasinoAgentBetSettingModel model);

        Task DeleteAgentBetSettingAsync(long id);

        string[] GetStringGeneralHandicaps(CasinoAgentBetSetting item);
    }
}
