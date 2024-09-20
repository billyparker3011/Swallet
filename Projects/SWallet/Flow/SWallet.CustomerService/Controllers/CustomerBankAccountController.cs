using HnMicro.Framework.Controllers;
using HnMicro.Framework.Responses;
using Microsoft.AspNetCore.Mvc;
using SWallet.Core.Models.Customers;
using SWallet.Core.Services.Customer;
using SWallet.CustomerService.Requests.Customer;

namespace SWallet.CustomerService.Controllers
{
    [Route(Core.Helpers.RouteHelper.V1.CustomerBankAccount.BaseRoute)]
    public class CustomerBankAccountController : HnControllerBase
    {
        private readonly ICustomerBankAccountService _customerBankAccountService;

        public CustomerBankAccountController(ICustomerBankAccountService customerBankAccountService)
        {
            _customerBankAccountService = customerBankAccountService;
        }

        [HttpGet]
        public async Task<IActionResult> GetCustomerBankAccount()
        {
            return Ok(OkResponse.Create(await _customerBankAccountService.GetCurrentCustomerBankAccounts()));
        }

        [HttpPost]
        public async Task<IActionResult> AddCustomerBankAccount([FromBody] AddOrUpdateCustomerBankAccountRequest request)
        {
            await _customerBankAccountService.AddOrUpdate(new AddOrUpdateCustomerBankAccountModel
            {
                NumberAccount = request.NumberAccount,
                CardHolder = request.CardHolder,
                BankId = request.BankId
            });
            return Ok();
        }
    }
}
