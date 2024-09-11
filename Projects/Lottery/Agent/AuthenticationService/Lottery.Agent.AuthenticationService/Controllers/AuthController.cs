using HnMicro.Framework.Controllers;
using Lottery.Agent.AuthenticationService.Requests.Auth;
using Lottery.Core.Models.Auth;
using Lottery.Core.Services.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Lottery.Agent.AuthenticationService.Controllers
{
    [AllowAnonymous]
    public class AuthController : HnControllerBase
    {
        private readonly IAgentAuthenticationService _authenticationService;

        public AuthController(IAgentAuthenticationService authenticationService)
        {
            _authenticationService = authenticationService;
        }

        [HttpPost]
        public async Task<IActionResult> Auth([FromBody] AuthRequest request)
        {
            //  Test.
            var token = await _authenticationService.Auth(new AuthModel { Username = request.Username, Password = request.Password });
            return Ok(token);
        }
    }
}