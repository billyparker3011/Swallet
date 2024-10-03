using HnMicro.Core.Scopes;
using Lottery.Core.Dtos.Agent;
using Lottery.Core.Models.Agent.GetAgentBetSettings;
using Lottery.Core.Models.Agent.GetCreditBalanceDetailPopup;
using Lottery.Core.Models.Player;
using Lottery.Core.Models.Player.CreatePlayer;
using Lottery.Core.Models.Player.UpdatePlayer;
using Lottery.Core.Models.Player.UpdatePlayerCreditBalance;

namespace Lottery.Core.Services.Player
{
    public interface IPlayerService : IScopedDependency
    {
        Task Logout();
        Task CreatePlayer(CreatePlayerModel model);
        Task UpdatePlayer(UpdatePlayerModel model);
        Task<string> GetSuggestionPlayerIdentifier();
        Task UpdatePlayerBetSetting(long playerId, List<AgentBetSettingDto> updateItems);
        Task UpdatePlayerCreditBalance(UpdatePlayerCreditBalanceModel updateItem);
        Task<GetCreditBalanceDetailPopupResult> GetCreditBalanceDetailPopup(long playerId);
        Task<GetAgentBetSettingsResult> GetDetailPlayerBetSettings(long playerId);

        Task<PlayerModel> GetPlayer(long playerId);
        Task CreateCockFightPlayer(CreatePlayerModel model);
    }
}
