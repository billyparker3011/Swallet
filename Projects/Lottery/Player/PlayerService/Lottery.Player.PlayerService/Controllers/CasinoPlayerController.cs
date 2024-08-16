using HnMicro.Framework.Controllers;
using Lottery.Core.Helpers;
using Lottery.Core.Partners.Models.Allbet;
using Lottery.Core.Services.Partners.CA;
using Microsoft.AspNetCore.Mvc;

namespace Lottery.Player.PlayerService.Controllers
{
    public class CasinoPlayerController : HnControllerBase
    {
        private readonly ICasinoService _caService;
        public CasinoPlayerController(ICasinoService cAService
            ) 
        {
            _caService = cAService;
        }

        [HttpPost("login")]
        public async Task<IActionResult> PlayerLogin(CasinoAllBetPlayerLoginModel model)
        {
            await _caService.AllBetPlayerLoginAsync(model);
            return Ok();
        }

        [HttpGet("get-url")]
        public async Task<IActionResult> GetUrl()
        {
            var result = await _caService.GetGameUrlAsync();
            return Ok(result);
        }

        [HttpGet("getbalance/{player}")]
        public async Task<IActionResult> GetBalance(string player)
        {
            return Ok(new CasinoBalanceResponseModel(PartnerHelper.CasinoReponseCode.Success, null, 2000, 1));
        }

        [HttpPost("transfer")]
        public async Task<IActionResult> Transfer(Transaction transaction)
        {
            return Ok(new CasinoBalanceResponseModel(PartnerHelper.CasinoReponseCode.Success, null, 2000 + transaction.Amount, 1));
        }

        [HttpPost("canceltransfer")]
        public async Task<IActionResult> CancelTransfer()
        {
            return Ok(new CasinoBalanceResponseModel(PartnerHelper.CasinoReponseCode.Success, null, 2000, 1));
        }
    }
}
