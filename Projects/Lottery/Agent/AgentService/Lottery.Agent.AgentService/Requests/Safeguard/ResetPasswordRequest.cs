namespace Lottery.Agent.AgentService.Requests.Safeguard
{
    public class ResetPasswordRequest
    {
        public string OldPassword { get; set; }
        public string Password { get; set; }
        public string ConfirmPassword { get; set; }
    }
}
