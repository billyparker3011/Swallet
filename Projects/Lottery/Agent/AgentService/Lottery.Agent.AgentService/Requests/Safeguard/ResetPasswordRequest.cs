namespace Lottery.Agent.AgentService.Requests.Safeguard
{
    public class ResetPasswordRequest
    {
        public string Password { get; set; }
        public string ConfirmPassword { get; set; }
    }
}
