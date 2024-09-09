using HnMicro.Framework.Controllers;
using HnMicro.Framework.Responses;
using Lottery.Core.Enums;
using Lottery.Core.Helpers;
using Lottery.Core.Partners.Attribute.CA;
using Lottery.Core.Partners.Attribute.CockFight;
using Lottery.Core.Partners.Helpers;
using Lottery.Core.Partners.Models.Allbet;
using Lottery.Core.Repositories.Casino;
using Lottery.Core.Services.Partners.CA;
using Lottery.Core.UnitOfWorks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using static Lottery.Core.Helpers.PartnerHelper;

namespace Lottery.Player.PlayerService.Controllers
{
    public class CasinoPlayerController : HnControllerBase
    {
        private readonly ICasinoService _casinoService;
        private readonly ICasinoGameTypeService _casinoGameTypeService;
        private readonly ICasinoTicketService _casinoTicketService;
        private readonly ICasinoPlayerMappingService _casinoPlayerMappingService;
        private readonly ICasinoRequestService _casinoRequestService;
        public CasinoPlayerController(ICasinoService casinoService,
            ICasinoGameTypeService casinoGameTypeService,
            ICasinoTicketService casinoTicketService,
            ICasinoPlayerMappingService casinoPlayerMappingService,
            ICasinoRequestService casinoRequestService
            ) 
        {
            _casinoGameTypeService = casinoGameTypeService;
            _casinoService = casinoService;
            _casinoTicketService = casinoTicketService;
            _casinoPlayerMappingService = casinoPlayerMappingService;
            _casinoRequestService = casinoRequestService;
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

        [HttpGet("game-types/{category}")]
        [Authorize(AuthenticationSchemes = nameof(CasinoAuthorizeAttribute))]
        public async Task<IActionResult> GetGameTypes([FromRoute] string category)
        {

            if (string.IsNullOrWhiteSpace(category)) return NotFound();
            return Ok(OkResponse.Create(await _casinoGameTypeService.GetGameTypesAsync(category)));
        }

        [HttpGet("game-types")]
        [Authorize(AuthenticationSchemes = nameof(CasinoAuthorizeAttribute))]
        public async Task<IActionResult> GetAllGameTypes()
        {
            return Ok(OkResponse.Create(await _casinoGameTypeService.GetAllGameTypesAsync()));
        }

        [HttpGet("getbalance/{player}")]
        [Authorize(AuthenticationSchemes = nameof(CasinoAuthorizeAttribute))]
        public async Task<IActionResult> GetBalance(string player)
        {
            var codeValidate = await _casinoRequestService.ValidateHeader(Request, null);

            if (codeValidate != CasinoReponseCode.Success) return Ok(new CasinoReponseModel(codeValidate));

            var playerMapping = await _casinoPlayerMappingService.FindPlayerMappingByBookiePlayerIdAsync(player);

            if (playerMapping == null) return Ok(new CasinoReponseModel(CasinoReponseCode.Player_account_does_not_exist));

            var balance = await _casinoTicketService.GetBalanceAsync(player, 0);         
            return Ok(new CasinoBalanceResponseModel(PartnerHelper.CasinoReponseCode.Success, null, balance, 1));
        }

        [HttpPost("transfer")]
        [Authorize(AuthenticationSchemes = nameof(CasinoAuthorizeAttribute))]
        public async Task<IActionResult> Transfer(object body)
        {
            var codeValidate = await _casinoRequestService.ValidateHeader(Request, body?.ToString());

            if (codeValidate != CasinoReponseCode.Success) return Ok(new CasinoReponseModel(codeValidate));

            CasinoTicketModel casinoTicketModel = JsonConvert.DeserializeObject<CasinoTicketModel>(body?.ToString());

            if (CasinoHelper.TypeTransfer.BetDetails.Contains(casinoTicketModel.Type) && casinoTicketModel.CasinoTicketBetDetailModels.Any(c=>!c.BetNum.ToString().Contains(c.GameRoundId.ToString())))
            {
                return Ok(new CasinoReponseModel(PartnerHelper.CasinoReponseCode.Transaction_not_existed));
            }
           (var balance, var code) = await _casinoTicketService.ProcessTicketAsync(casinoTicketModel, 0);

            if (code != CasinoReponseCode.Success) return Ok(new CasinoReponseModel(code));

            return Ok(new CasinoBalanceResponseModel(PartnerHelper.CasinoReponseCode.Success, null, balance, 1));
        }

        [HttpPost("canceltransfer")]
        [Authorize(AuthenticationSchemes = nameof(CasinoAuthorizeAttribute))]
        public async Task<IActionResult> CancelTransfer(object body)
        {
            var codeValidate = await _casinoRequestService.ValidateHeader(Request, body?.ToString());

            if (codeValidate != CasinoReponseCode.Success) return Ok(new CasinoReponseModel(codeValidate));

            CasinoCancelTicketModel casinoCancelTicketModel = JsonConvert.DeserializeObject<CasinoCancelTicketModel>(body?.ToString());

            if (casinoCancelTicketModel.OriginalDetails.Any(c => !c.BetNum.ToString().Contains(c.GameRoundId.ToString())))
            {
                return Ok(new CasinoReponseModel(PartnerHelper.CasinoReponseCode.Transaction_not_existed));
            }

            (var balance, var code) = await _casinoTicketService.ProcessCancelTicketAsync(casinoCancelTicketModel, 0);

            if (code != CasinoReponseCode.Success) return Ok(new CasinoReponseModel(code));

            return Ok(new CasinoBalanceResponseModel(PartnerHelper.CasinoReponseCode.Success, null, balance, 1));
        }
    }
}
