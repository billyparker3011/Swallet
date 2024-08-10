using Lottery.Core.Helpers;
using Lottery.Core.Partners.Attribute.CA;
using Lottery.Core.Partners.Core;
using Lottery.Core.Partners.Models.CA;
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
            return Ok(new CABalanceResponseModel(PartnerHelper.CAReponseCode.Success, null, 2000, 1));
        }

        [HttpPost("Transfer")]
        public async Task<IActionResult> Transfer()
        {
            return Ok(new CABalanceResponseModel(PartnerHelper.CAReponseCode.Success, null, new Random().NextInt64(1000, 2500), 1));
        }
        
        [HttpPost("CancelTransfer")]
        public async Task<IActionResult> CancelTransfer()
        {
            return Ok(new CABalanceResponseModel(PartnerHelper.CAReponseCode.Success, null, new Random().NextInt64(1000, 2000), 1));
        }


    }
}
