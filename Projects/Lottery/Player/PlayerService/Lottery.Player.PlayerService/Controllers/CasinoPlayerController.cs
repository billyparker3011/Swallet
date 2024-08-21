using HnMicro.Framework.Controllers;
using HnMicro.Framework.Responses;
using Lottery.Core.Enums;
using Lottery.Core.Helpers;
using Lottery.Core.Partners.Attribute.CA;
using Lottery.Core.Partners.Attribute.CockFight;
using Lottery.Core.Partners.Models.Allbet;
using Lottery.Core.Services.Partners.CA;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Lottery.Player.PlayerService.Controllers
{
    public class CasinoPlayerController : HnControllerBase
    {
        private readonly ICasinoService _casinoService;
        private readonly ICasinoGameTypeService _casinoGameTypeService;
        private readonly ICasinoTicketService _casinoTicketService;
        public CasinoPlayerController(ICasinoService casinoService,
            ICasinoGameTypeService casinoGameTypeService,
            ICasinoTicketService casinoTicketService
            ) 
        {
            _casinoGameTypeService = casinoGameTypeService;
            _casinoService = casinoService;
            _casinoTicketService = casinoTicketService;
        }

        [HttpPost("login")]
        public async Task<IActionResult> PlayerLogin(CasinoAllBetPlayerLoginModel model)
        {
            await _casinoService.AllBetPlayerLoginAsync(model);
            return Ok();
        }

        [HttpGet("get-url")]
        public async Task<IActionResult> GetUrl()
        {
            var result = await _casinoService.GetGameUrlAsync();
            return Ok(result);
        }

        [HttpGet("game-types/{caterory}")]
        public async Task<IActionResult> GetGameTypes([FromRoute] string caterory)
        {

            if (string.IsNullOrWhiteSpace(caterory)) return NotFound();
            return Ok(OkResponse.Create(await _casinoGameTypeService.GetGameTypesAsync(caterory)));
        }

        [HttpGet("game-types")]
        public async Task<IActionResult> GetAllGameTypes()
        {
            return Ok(OkResponse.Create(await _casinoGameTypeService.GetAllGameTypesAsync()));
        }

        [HttpGet("getbalance/{player}")]
        [Authorize(AuthenticationSchemes = nameof(CasinoAuthorizeAttribute))]
        public async Task<IActionResult> GetBalance(string player)
        {
            return Ok(new CasinoBalanceResponseModel(PartnerHelper.CasinoReponseCode.Success, null, 2000, 1));
        }

        [HttpPost("transfer")]
        [Authorize(AuthenticationSchemes = nameof(CasinoAuthorizeAttribute))]
        public async Task<IActionResult> Transfer(CasinoTicketModel casinoTicketModel)
        {
           var balance = await _casinoTicketService.ProcessTicketAsync(casinoTicketModel, 2000);
            return Ok(new CasinoBalanceResponseModel(PartnerHelper.CasinoReponseCode.Success, null, balance, 1));
        }

        [HttpPost("canceltransfer")]
        [Authorize(AuthenticationSchemes = nameof(CasinoAuthorizeAttribute))]
        public async Task<IActionResult> CancelTransfer(CasinoCancelTicketModel casinoCancelTicketModel)
        {
            var balance = await _casinoTicketService.ProcessCancelTicketAsync(casinoCancelTicketModel, 2000);
            return Ok(new CasinoBalanceResponseModel(PartnerHelper.CasinoReponseCode.Success, null, balance, 1));
        }
    }
}
