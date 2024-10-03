using HnMicro.Core.Scopes;
using Lottery.Core.Models.CockFight.GetCockFightAgentBetSetting;
using Lottery.Core.Models.CockFight.UpdateCockFightAgentBetSetting;

namespace Lottery.Core.Services.CockFight
{
    public interface ICockFightAgentBetSettingService : IScopedDependency
    {
        Task<GetCockFightAgentBetSettingResult> GetCockFightAgentBetSettingDetail(long agentId);
        Task UpdateCockFightAgentBetSetting(long agentId, UpdateCockFightAgentBetSettingModel model);
        Task<GetCockFightAgentBetSettingResult> GetDefaultCockFightCompanyBetSetting();
        Task UpdateDefaultCockFightCompanyBetSetting(UpdateCockFightAgentBetSettingModel model);
    }
}
