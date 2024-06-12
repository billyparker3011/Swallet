using Lottery.Core.Dtos.Agent;

namespace Lottery.Player.PlayerService.Requests.Player
{
    public class UpdatePlayerBetSettingRequest
    {
        public List<AgentBetSettingDto> BetSettings { get; set; }
    }
}
