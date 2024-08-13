using Lottery.Core.Helpers;
using Lottery.Core.Partners.Attribute.CA;
using Lottery.Core.Partners.Core;
using Lottery.Core.Partners.Models.Allbet;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Lottery.Player.PlayerService.Controllers
{
    [Authorize(AuthenticationSchemes = nameof(CAAuthorizeAttribute))]
    public class AllBetController : PartnerControllerBase
    {
        [HttpGet("GetBalance/{player}")]
        public async Task<IActionResult> GetBalance(string player)
        {
            return Ok(new CasinoBalanceResponseModel(PartnerHelper.CAReponseCode.Success, null, 2000, 1));
        }

        [HttpPost("Transfer")]
        public async Task<IActionResult> Transfer(Transaction transaction)
        {
            return Ok(new CasinoBalanceResponseModel(PartnerHelper.CAReponseCode.Success, null, 2000 + transaction.Amount, 1));
        }

        [HttpPost("CancelTransfer")]
        public async Task<IActionResult> CancelTransfer()
        {
            return Ok(new CasinoBalanceResponseModel(PartnerHelper.CAReponseCode.Success, null, 2000, 1));
        }
    }
}
