using HnMicro.Framework.Controllers;
using HnMicro.Framework.Responses;
using Microsoft.AspNetCore.Mvc;
using SWallet.Core.Services.Customer;

namespace SWallet.CustomerService.Controllers
{
    public class BalanceController : HnControllerBase
    {
        private readonly ICustomerService _customerService;

        public BalanceController(ICustomerService customerService)
        {
            _customerService = customerService;
        }

        [HttpGet]
        public async Task<IActionResult> MyBalance()
        {
            return Ok(OkResponse.Create(await _customerService.MyBalance()));
        }
    }
}
