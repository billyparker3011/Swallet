using Lottery.Core.Dtos.Agent;

namespace Lottery.Agent.AgentService.Requests.Agent
{
    public class CreateAgentRequest
    {
        public string Username { get; set; }
        public string Password { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public decimal Credit { get; set; }
        public decimal? MemberMaxCredit { get; set; }
        public List<AgentBetSettingDto> BetSettings { get; set; } = new List<AgentBetSettingDto>();
        public List<AgentPositionTakingDto> PositionTakings { get; set; } = new List<AgentPositionTakingDto>();
    }
}
