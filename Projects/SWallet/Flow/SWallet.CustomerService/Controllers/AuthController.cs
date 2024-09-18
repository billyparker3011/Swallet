using HnMicro.Framework.Controllers;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SWallet.Core.Models.Auth;
using SWallet.Core.Services.Auth;
using SWallet.CustomerService.Requests.Auth;

namespace SWallet.CustomerService.Controllers
{
    [AllowAnonymous]
    public class AuthController : HnControllerBase
    {
        private readonly ICustomerAuthenticationService _customerAuthenticationService;

        public AuthController(ICustomerAuthenticationService customerAuthenticationService)
        {
            _customerAuthenticationService = customerAuthenticationService;
        }

        [HttpPost]
        public async Task<IActionResult> Auth([FromBody] AuthRequest request)
        {
            var token = await _customerAuthenticationService.Auth(new AuthModel { Username = request.Username, Password = request.Password });
            return Ok(token);
        }
    }
}
