using Lottery.Core.Dtos.Agent;

namespace Lottery.Core.Models.Agent.CreateAgent
{
    public class CreateAgentModel
    {
        public string Username { get; set; }
        public string Password { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public decimal Credit { get; set; }
        public decimal? MemberMaxCredit { get; set; }
        public List<AgentBetSettingDto> BetSettings { get; set; }
        public List<AgentPositionTakingDto> PositionTakings { get; set; }
    }
}
