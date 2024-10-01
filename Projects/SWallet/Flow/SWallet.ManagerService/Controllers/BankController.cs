using HnMicro.Framework.Controllers;
using HnMicro.Framework.Responses;
using Microsoft.AspNetCore.Mvc;
using SWallet.Core.Models;
using SWallet.Core.Models.Bank.GetBanks;
using SWallet.Core.Services.Bank;
using SWallet.ManagerService.Requests;

namespace SWallet.ManagerService.Controllers
{
    public class BankController : HnControllerBase
    {
        private readonly IBankService _bankService;

        public BankController(IBankService bankService)
        {
            _bankService = bankService;
        }

        [HttpGet]
        public async Task<IActionResult> GetBanks([FromQuery] GetBanksRequest request)
        {
            return Ok(OkResponse.Create(await _bankService.GetBanks(new GetBanksModel
            {
                SearchName = request.SearchName,
                PageIndex = request.PageIndex,
                PageSize = request.PageSize,
                SortName = request.SortName,
                SortType = request.SortType
            })));
        }

        [HttpPost]
        public async Task<IActionResult> CreateBank([FromBody] CreateBankRequest request)
        {
            await _bankService.CreateBank(new CreateBankModel
            {
                Name = request.Name,
                Icon = request.Icon,
                DepositEnabled = request.DepositEnabled,
                WithdrawEnabled = request.WithdrawEnabled
            });
            return Ok();
        }

        [HttpPut("{id:int}")]
        public async Task<IActionResult> UpdateBank([FromRoute] int id, [FromBody] CreateBankRequest request)
        {
            await _bankService.UpdateBank(id, new CreateBankModel
            {
                Name = request.Name,
                Icon = request.Icon,
                DepositEnabled = request.DepositEnabled,
                WithdrawEnabled = request.WithdrawEnabled
            });
            return Ok();
        }

        [HttpDelete("{id:int}")]
        public async Task<IActionResult> DeleteBank([FromRoute] int id)
        {
            await _bankService.DeleteBank(id);
            return Ok();
        }

        [HttpGet("exists")]
        public async Task<IActionResult> CheckExistBank([FromQuery] string bankName, [FromQuery] int? bankId)
        {
            return Ok(OkResponse.Create(await _bankService.CheckExistBank(bankName, bankId)));
        }

        [HttpGet("active-banks")]
        public async Task<IActionResult> ActiveBanks()
        {
            return Ok(OkResponse.Create(await _bankService.ActiveBanks()));
        }
    }
}