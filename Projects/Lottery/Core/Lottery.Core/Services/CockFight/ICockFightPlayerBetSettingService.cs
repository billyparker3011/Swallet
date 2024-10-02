using HnMicro.Core.Scopes;
using Lottery.Core.Models.CockFight.GetCockFightAgentBetSetting;
using Lottery.Core.Models.CockFight.UpdateCockFightAgentBetSetting;

namespace Lottery.Core.Services.CockFight
{
    public interface ICockFightPlayerBetSettingService : IScopedDependency
    {
        Task<GetCockFightAgentBetSettingResult> GetCockFightPlayerBetSettingDetail(long playerId);
        Task UpdateCockFightPlayerBetSetting(long playerId, UpdateCockFightAgentBetSettingModel model);
    }
}
