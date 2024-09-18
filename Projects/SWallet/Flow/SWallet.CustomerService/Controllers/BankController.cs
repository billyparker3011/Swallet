using HnMicro.Framework.Controllers;
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
            return Ok(await _bankService.GetBankBy(true, false));
        }

        [HttpGet("withdraw-banks")]
        public async Task<IActionResult> WithdrawBanks()
        {
            return Ok(await _bankService.GetBankBy(false, true));
        }
    }
}
