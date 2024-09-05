using HnMicro.Framework.Controllers;
using Lottery.Core.Partners.Attribute.Bti;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json.Linq;
using static Azure.Core.HttpHeader;
using static Lottery.Core.Helpers.AuditDataHelper;
using System.Diagnostics;
using Lottery.Core.Partners.Models.Bti;

namespace Lottery.Player.PlayerService.Controllers
{    
    public class BtiPlayerController : HnControllerBase
    {
        public BtiPlayerController(
        )
        {
        }

        [HttpGet("validatetoken")]
        [Authorize(AuthenticationSchemes = nameof(BtiAuthorizeAttribute))]
        public async Task<IActionResult> ValidateToken()
        {
            return Ok(new BtiValidateTokenResponseModel());
        }

        [HttpPost("reserve")]
        [Authorize(AuthenticationSchemes = nameof(BtiAuthorizeAttribute))]
        public async Task<IActionResult> Reserve()
        {
            return Ok(new BtiReserveResponseModel());
        }

        [HttpPost("debitreserve")]
        [Authorize(AuthenticationSchemes = nameof(BtiAuthorizeAttribute))]
        public async Task<IActionResult> DebitReserve()
        {
            return Ok(new BtiDebitReserveResponseModel());
        }

        [HttpGet("cancelreserve")]
        [Authorize(AuthenticationSchemes = nameof(BtiAuthorizeAttribute))]
        public async Task<IActionResult> CancelReserve()
        {
            return Ok(new BtiBaseResponseModel());
        }

        [HttpGet("commitreserve")]
        [Authorize(AuthenticationSchemes = nameof(BtiAuthorizeAttribute))]
        public async Task<IActionResult> CommitReserve()
        {
            return Ok(new BtiBaseResponseModel());
        }

        [HttpPost("debitcustomer")]
        [Authorize(AuthenticationSchemes = nameof(BtiAuthorizeAttribute))]
        public async Task<IActionResult> DebitCustomer()
        {
            return Ok(new BtiBaseResponseModel());
        }

        [HttpPost("creditcustomer")]
        [Authorize(AuthenticationSchemes = nameof(BtiAuthorizeAttribute))]
        public async Task<IActionResult> CreditCustomer()
        {
            return Ok(new BtiBaseResponseModel());
        }

        [HttpGet("btisport.js")]
        [Authorize(AuthenticationSchemes = nameof(BtiAuthorizeAttribute))]
        public async Task<IActionResult> BtiSport()
        {
            return Ok();
        }

    }
}
