using HnMicro.Framework.Controllers;
using HnMicro.Framework.Responses;
using Microsoft.AspNetCore.Mvc;
using SWallet.Core.Services.Bank;

namespace SWallet.CustomerService.Controllers
{
    public class BankController : HnControllerBase
    {
        private readonly IBankService _bankService;

        public BankController(IBankService bankService)
        {
            _bankService = bankService;
        }

        [HttpGet("deposit-banks")]
        public async Task<IActionResult> DepositBanks()
        {
            return Ok(OkResponse.Create(await _bankService.GetBankBy(true, false)));
        }

        [HttpGet("withdraw-banks")]
        public async Task<IActionResult> WithdrawBanks()
        {
            return Ok(OkResponse.Create(await _bankService.GetBankBy(false, true)));
        }

        [HttpGet("active-banks")]
        public async Task<IActionResult> ActiveBanks()
        {
            return Ok(OkResponse.Create(await _bankService.ActiveBanks()));
        }
    }
}
