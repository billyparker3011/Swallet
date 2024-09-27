using HnMicro.Framework.Controllers;
using HnMicro.Framework.Responses;
using Microsoft.AspNetCore.Mvc;
using SWallet.Core.Models;
using SWallet.Core.Services.Bank;
using SWallet.ManagerService.Requests;

namespace SWallet.ManagerService.Controllers
{
    public class BankAccountController : HnControllerBase
    {
        private readonly IBankAccountService _bankAccountService;

        public BankAccountController(IBankAccountService bankAccountService)
        {
            _bankAccountService = bankAccountService;
        }

        [HttpGet]
        public async Task<IActionResult> GetBanksAccount()
        {
            return Ok(OkResponse.Create(await _bankAccountService.GetBankAccounts()));
        }

        [HttpPost]
        public async Task<IActionResult> CreateBankAccount([FromBody] CreateBankAccountRequest request)
        {
            await _bankAccountService.CreateBankAccount(new CreateBankAccountModel
            {
                BankId = request.BankId,
                NumberAccount = request.NumberAccount,
                CardHolder = request.CardHolder
            });
            return Ok();
        }

        [HttpPut("{id:int}")]
        public async Task<IActionResult> UpdateBankAccount([FromRoute] int id, [FromBody] CreateBankAccountRequest request)
        {
            await _bankAccountService.UpdateBankAccount(id, new CreateBankAccountModel
            {
                BankId = request.BankId,
                NumberAccount = request.NumberAccount,
                CardHolder = request.CardHolder
            });
            return Ok();
        }

        [HttpDelete("{id:int}")]
        public async Task<IActionResult> DeleteBankAccount([FromRoute] int id)
        {
            await _bankAccountService.DeleteBankAccount(id);
            return Ok();
        }

        [HttpGet("exists")]
        public async Task<IActionResult> CheckExistBankAccount([FromQuery] int bankId, [FromQuery] string accountNumber, [FromQuery] int? bankAccountId)
        {
            return Ok(OkResponse.Create(await _bankAccountService.CheckExistBankAccount(bankId, accountNumber, bankAccountId)));
        }
    }
}