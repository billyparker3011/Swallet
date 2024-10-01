using HnMicro.Framework.Controllers;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SWallet.Core.Models.Auth;
using SWallet.Core.Services.Auth;
using SWallet.ManagerService.Requests.Auth;

namespace SWallet.ManagerService.Controllers
{
    [AllowAnonymous]
    public class AuthController : HnControllerBase
    {
        private readonly IManagerAuthenticationService _managerAuthenticationService;

        public AuthController(IManagerAuthenticationService managerAuthenticationService)
        {
            _managerAuthenticationService = managerAuthenticationService;
        }

        [HttpPost]
        public async Task<IActionResult> Auth([FromBody] AuthRequest request)
        {
            var token = await _managerAuthenticationService.Auth(new AuthModel { Username = request.Username, Password = request.Password });
            return Ok(token);
        }
    }
}