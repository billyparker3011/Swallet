using HnMicro.Framework.Controllers;
using Microsoft.AspNetCore.Mvc;
using SWallet.Core.Models.Customers;
using SWallet.Core.Services.Customer;
using SWallet.CustomerService.Requests.Customer;

namespace SWallet.CustomerService.Controllers
{
    public class CustomerController : HnControllerBase
    {
        private readonly IRegistrationService _registrationService;

        public CustomerController(IRegistrationService registrationService)
        {
            _registrationService = registrationService;
        }

        [HttpPost]
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
    }
}
