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
        [HttpGet("GetBalance")]
        public async Task<IActionResult> GetBalance([FromQuery] string player)
        {
            return Ok(new CABalanceResponseModel(PartnerHelper.CAReponseCode.Success, null, 2000, 1));
        }
    }
}
