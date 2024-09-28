using HnMicro.Framework.Controllers;
using HnMicro.Framework.Responses;
using Microsoft.AspNetCore.Mvc;
using SWallet.Core.Services.Customer;

namespace SWallet.ManagerService.Controllers
{
    public class CustomerController : HnControllerBase
    {
        private readonly ICustomerService _customerService;

        public CustomerController(ICustomerService customerService)
        {
            _customerService = customerService;
        }

        [HttpGet("{profileId:long}/profile")]
        public async Task<IActionResult> Profile([FromRoute] long customerId)
        {
            return Ok(OkResponse.Create(await _customerService.CustomerProfile(customerId)));
        }
    }
}