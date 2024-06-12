using HnMicro.Framework.Controllers;
using Lottery.Agent.AgentService.Requests.Safeguard;
using Lottery.Core.Services.Agent;
using Microsoft.AspNetCore.Mvc;

namespace Lottery.Agent.AgentService.Controllers
{
    public class SafeguardController : HnControllerBase
    {
        private readonly IAgentSafeguardService _securityService;

        public SafeguardController(IAgentSafeguardService securityService)
        {
            _securityService = securityService;
        }

        [HttpPut("{agentId}/reset-password")]
        public async Task<IActionResult> ResetPassword([FromRoute] long agentId, [FromBody] ResetPasswordRequest request)
        {
            await _securityService.ResetPassword(new Core.Models.Auth.ResetPasswordModel { TargetId = agentId, Password = request.Password, ConfirmPassword = request.ConfirmPassword });
            return Ok();
        }

        [HttpPut("{agentId:long}/reset-sercurity-code")]
        public async Task<IActionResult> ResetSecurityCode([FromRoute] long agentId, [FromBody] ResetSecurityCodeRequest request)
        {
            await _securityService.ResetSercurityCode(new Core.Models.Auth.ResetSecurityCodeModel { TargetId = agentId, SecurityCode = request.SecurityCode, ConfirmSecurityCode = request.ConfirmSecurityCode });
            return Ok();
        }

        [HttpPut("reset-login-user-password")]
        public async Task<IActionResult> ResetLoginUserPassword([FromBody] ResetPasswordRequest request)
        {
            await _securityService.ResetLoginUserPassword(request.Password, request.ConfirmPassword);
            return Ok();
        }

        [HttpPut("reset-login-user-sercurity-code")]
        public async Task<IActionResult> ResetLoginUserSecurityCode([FromBody] ResetSecurityCodeRequest request)
        {
            await _securityService.ResetLoginUserSercurityCode(request.SecurityCode, request.ConfirmSecurityCode);
            return Ok();
        }
    }
}
