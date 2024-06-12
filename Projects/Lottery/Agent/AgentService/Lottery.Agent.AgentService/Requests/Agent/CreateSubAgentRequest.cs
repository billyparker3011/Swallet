namespace Lottery.Agent.AgentService.Requests.Agent
{
    public class CreateSubAgentRequest
    {
        public string Username { get; set; }
        public string Password { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Permissions { get; set; }
    }
}
