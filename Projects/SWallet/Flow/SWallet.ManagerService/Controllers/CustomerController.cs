using HnMicro.Framework.Controllers;
using HnMicro.Framework.Responses;
using Microsoft.AspNetCore.Mvc;
using SWallet.Core.Models.Auth;
using SWallet.Core.Models.Customers;
using SWallet.Core.Services.Auth;
using SWallet.Core.Services.Customer;
using SWallet.ManagerService.Requests;

namespace SWallet.ManagerService.Controllers
{
    public class CustomerController : HnControllerBase
    {
        private readonly ICustomerService _customerService;
        private readonly IPasswordService _passwordService;

        public CustomerController(ICustomerService customerService, IPasswordService passwordService)
        {
            _customerService = customerService;
            _passwordService = passwordService;
        }

        [HttpGet("{profileId:long}/profile")]
        public async Task<IActionResult> Profile([FromRoute] long customerId)
        {
            return Ok(OkResponse.Create(await _customerService.CustomerProfile(customerId)));
        }

        [HttpPost("change-password")]
        public async Task<IActionResult> ChangePassword([FromBody] ChangePasswordRequest request)
        {
            await _passwordService.ChangeCustomerPassword(new ChangePasswordModel
            {
                CustomerId = request.CustomerId,
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
                CustomerId = request.CustomerId,
                LastName = request.LastName,
                FirstName = request.FirstName,
                Phone = request.Phone,
                Telegram = request.Telegram,
                IsLock = request.IsLock,
                State = request.State
            });
            return Ok();
        }
    }
}