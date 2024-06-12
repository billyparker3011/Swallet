using HnMicro.Core.Scopes;
using Lottery.Core.Models.Setting;

namespace Lottery.Core.Services.Player
{
    public interface IPlayerSettingService : IScopedDependency
    {
        Task BuildSettingByBetKindCache(long playerId, int betKindId, BetSettingModel setting);
        Task BuildSettingByBetKindCache(long playerId, Dictionary<int, BetSettingModel> settings);
        Task<(BetSettingModel, bool)> GetBetSettings(long playerId, int betKindId);
        Task<(Dictionary<int, BetSettingModel>, bool)> GetBetSettings(long playerId, List<int> betKindIds);
        Task<PlayerBetSettingModel> GetMyBetSettingByBetKind(int betKindId);
        Task<Dictionary<int, BetSettingModel>> GetMyMixedBetSettingByBetKind(int betKindId);
    }
}
