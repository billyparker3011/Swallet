using HnMicro.Framework.Controllers;
using Lottery.Core.Partners.Attribute.Bti;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Lottery.Core.Services.Partners.Bti;

namespace Lottery.Player.PlayerService.Controllers
{    
    public class BtiPlayerController : HnControllerBase
    {
        private readonly IBtiSerivice _btiSerivice;
        private readonly IBtiTicketService _btiTicketService;

        public BtiPlayerController(
            IBtiSerivice btiSerivice,
            IBtiTicketService btiTicketService
        )
        {
            _btiSerivice = btiSerivice;
            _btiTicketService = btiTicketService;
        }

        [HttpGet("gettoken")]
        [Authorize(AuthenticationSchemes = nameof(BtiAuthorizeAttribute))]
        public async Task<IActionResult> GetToken(long playerId)
        {
            return Ok(_btiSerivice.GenerateToken(playerId, DateTime.UtcNow.AddDays(1400)));
        }

        [HttpGet("validatetoken")]
        [Authorize(AuthenticationSchemes = nameof(BtiAuthorizeAttribute))]
        public async Task<IActionResult> ValidateToken([FromQuery] string auth_token)
        {
            return Ok(await _btiTicketService.ValidateToken(auth_token));
        }

        [HttpPost("reserve")]
        [Authorize(AuthenticationSchemes = nameof(BtiAuthorizeAttribute))]
        public async Task<IActionResult> Reserve(string cust_id, long reserve_id, decimal amount, string extsessionID)
        {
            var requestBody = await GetRequestBody(Request.Body);
            var result = await _btiTicketService.Reverse(cust_id, reserve_id, amount, extsessionID, requestBody);
            return Ok(result);
        }

        [HttpPost("debitreserve")]
        [Authorize(AuthenticationSchemes = nameof(BtiAuthorizeAttribute))]
        public async Task<IActionResult> DebitReserve(string cust_id, long reserve_id, decimal amount, long req_id, long purchase_id)
        {
            var requestBody = await GetRequestBody(Request.Body);
            var result = await _btiTicketService.DebitReverse(cust_id, reserve_id, amount, req_id, purchase_id, requestBody);
            return Ok(result);
        }

        [HttpGet("cancelreserve")]
        [Authorize(AuthenticationSchemes = nameof(BtiAuthorizeAttribute))]
        public async Task<IActionResult> CancelReserve(string cust_id, long reserve_id)
        {
            var result = await _btiTicketService.CancelReverse(cust_id, reserve_id);
            return Ok(result);
        }

        [HttpGet("commitreserve")]
        [Authorize(AuthenticationSchemes = nameof(BtiAuthorizeAttribute))]
        public async Task<IActionResult> CommitReserve(string cust_id, long reserve_id, long purchase_id)
        {
            var result = await _btiTicketService.CommitReverse(cust_id, reserve_id, purchase_id);
            return Ok(result);
        }

        [HttpPost("debitcustomer")]
        [Authorize(AuthenticationSchemes = nameof(BtiAuthorizeAttribute))]
        public async Task<IActionResult> DebitCustomer(string cust_id, decimal amount, long req_id, long purchase_id)
        {
            var requestBody = await GetRequestBody(Request.Body);
            var result = await _btiTicketService.DebitCustomer(cust_id, amount, req_id, purchase_id, requestBody);
            return Ok(result);
        }

        [HttpPost("creditcustomer")]
        [Authorize(AuthenticationSchemes = nameof(BtiAuthorizeAttribute))]
        public async Task<IActionResult> CreditCustomer(string cust_id, decimal amount, long req_id, long purchase_id)
        {
            var requestBody = await GetRequestBody(Request.Body);
            var result = await _btiTicketService.CreditCustomer(cust_id, amount, req_id, purchase_id, requestBody);
            return Ok(result);
        }

        [HttpGet("btisport.js")]
        [Authorize(AuthenticationSchemes = nameof(BtiAuthorizeAttribute))]
        public async Task<IActionResult> BtiSport()
        {
            return Ok();
        }

        private async Task<string> GetRequestBody(Stream body)
        {
            if (body == null) return string.Empty;
            using (var reader = new StreamReader(body))
            {
                return await reader.ReadToEndAsync();
            }
        }
    }
}
