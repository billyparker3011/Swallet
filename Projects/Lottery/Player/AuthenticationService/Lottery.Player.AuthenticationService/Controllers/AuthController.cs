using HnMicro.Framework.Controllers;
using Lottery.Core.Services.Authentication;
using Lottery.Player.AuthenticationService.Requests.Auth;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Lottery.Player.AuthenticationService.Controllers
{
    [AllowAnonymous]
    public class AuthController : HnControllerBase
    {
        private readonly IPlayerAuthenticationService _playerAuthenticationService;

        public AuthController(IPlayerAuthenticationService playerAuthenticationService)
        {
            _playerAuthenticationService = playerAuthenticationService;
        }

        [HttpPost]
        public async Task<IActionResult> Auth([FromBody] AuthRequest request)
        {
            var token = await _playerAuthenticationService.Auth(new Core.Models.Auth.AuthModel { Username = request.Username, Password = request.Password });
            return Ok(token);
        }
    }
}