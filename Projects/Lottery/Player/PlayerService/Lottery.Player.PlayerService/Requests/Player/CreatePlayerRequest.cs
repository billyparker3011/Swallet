using Lottery.Core.Dtos.Agent;

namespace Lottery.Player.PlayerService.Requests.Player
{
    public class CreatePlayerRequest
    {
        public string Username { get; set; }
        public string Password { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public decimal Credit { get; set; }
        public decimal? MemberMaxCredit { get; set; }
        public List<AgentBetSettingDto> BetSettings { get; set; } = new List<AgentBetSettingDto>();
    }
}
