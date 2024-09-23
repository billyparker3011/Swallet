using HnMicro.Framework.Controllers;
using HnMicro.Framework.Responses;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SWallet.Core.Models.Auth;
using SWallet.Core.Models.Customers;
using SWallet.Core.Services.Auth;
using SWallet.Core.Services.Customer;
using SWallet.CustomerService.Requests.Customer;

namespace SWallet.CustomerService.Controllers
{
    public class CustomerController : HnControllerBase
    {
        private readonly IRegistrationService _registrationService;
        private readonly IPasswordService _passwordService;
        private readonly ICustomerService _customerService;

        public CustomerController(IRegistrationService registrationService, IPasswordService passwordService, ICustomerService customerService)
        {
            _registrationService = registrationService;
            _passwordService = passwordService;
            _customerService = customerService;
        }

        [HttpPost("register"), AllowAnonymous]
        public async Task<IActionResult> Register([FromBody] RegisterRequest request)
        {
            var token = await _registrationService.Register(new RegisterModel
            {
                Accepted = request.Accepted,
                AffiliateCode = request.AffiliateCode,
                DiscountId = request.DiscountId,
                Email = request.Email,
                FirstName = request.FirstName,
                LastName = request.LastName,
                Password = request.Password,
                Username = request.Username,
                Telegram = request.Telegram,
                Phone = request.Phone
            });
            return Ok(token);
        }

        [HttpPost("change-password")]
        public async Task<IActionResult> ChangePassword([FromBody] ChangePasswordRequest request)
        {
            await _passwordService.ChangeCustomerPassword(new ChangePasswordModel
            {
                ConfirmPassword = request.ConfirmPassword,
                NewPassword = request.NewPassword,
                OldPassword = request.OldPassword
            });
            return Ok();
        }

        [HttpPost("change-info")]
        public async Task<IActionResult> ChangeInfo([FromBody] ChangeInfoRequest request)
        {
            await _customerService.ChangeInfo(new ChangeInfoModel
            {
                LastName = request.LastName,
                FirstName = request.FirstName,
                Phone = request.Phone,
                Telegram = request.Telegram
            });
            return Ok();
        }

        [HttpGet("my-profile")]
        public async Task<IActionResult> MyProfile()
        {
            return Ok(OkResponse.Create(await _customerService.MyProfile()));
        }
    }
}
