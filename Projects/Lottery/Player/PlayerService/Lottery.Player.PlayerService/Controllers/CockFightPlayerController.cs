using HnMicro.Core.Helpers;
using HnMicro.Framework.Controllers;
using HnMicro.Framework.Responses;
using HnMicro.Modules.LoggerService.Services;
using Lottery.Core.Enums;
using Lottery.Core.Models.CockFight.UpdateCockFightAgentBetSetting;
using Lottery.Core.Partners.Attribute.CockFight;
using Lottery.Core.Services.CockFight;
using Lottery.Player.PlayerService.Helpers.Converters;
using Lottery.Player.PlayerService.Requests.CockFight;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Lottery.Player.PlayerService.Controllers
{
    public class CockFightPlayerController : HnControllerBase
    {
        private readonly ICockFightService _cockFightService;
        private readonly ICockFightPlayerBetSettingService _cockFightPlayerBetSettingService;
        private readonly ILoggerService _loggerService;

        public CockFightPlayerController(ICockFightService cockFightService, ICockFightPlayerBetSettingService cockFightPlayerBetSettingService, ILoggerService loggerService)
        {
            _cockFightService = cockFightService;
            _cockFightPlayerBetSettingService = cockFightPlayerBetSettingService;
            _loggerService = loggerService;
        }

        [HttpPost("initiate-player")]
        public async Task<IActionResult> InitiateCockFightPlayer()
        {
            await _cockFightService.CreateCockFightPlayer();
            return Ok();
        }

        [HttpPost("login")]
        public async Task<IActionResult> LoginCockFightPlayer()
        {
            await _cockFightService.LoginCockFightPlayer();
            return Ok();
        }

        [HttpGet("get-url")]
        public async Task<IActionResult> GetCockFightUrl()
        {
            return Ok(OkResponse.Create(await _cockFightService.GetCockFightUrl()));
        }

        [HttpGet("get-balance")]
        [Authorize(AuthenticationSchemes = nameof(CockFightAuthorizeAttribute))]
        public async Task<IActionResult> GetCockFightPlayerBalance([FromQuery] string member_ref_id)
        {
            return Ok(await _cockFightService.GetCockFightPlayerBalance(member_ref_id));
        }

        [HttpPost("transfer")]
        [Authorize(AuthenticationSchemes = nameof(CockFightAuthorizeAttribute))]
        public async Task<IActionResult> TransferTicket([FromBody] TransferTicketRequest request)
        {
            await _loggerService.Info(new HnMicro.Framework.Logger.Models.LogRequestModel
            {
                CategoryName = "TransferCockFightPlayerTickets",
                Message = "TransferCockFightPlayerTickets Request",
                Stacktrace = Newtonsoft.Json.JsonConvert.SerializeObject(request),
                RoleId = Role.Player.ToInt(),
                CreatedBy = 0L
            });
            await _cockFightService.TransferCockFightPlayerTickets(request.ToGa28TransferTicketModel());
            return Ok();
        }

        [HttpGet("{playerId:long}/bet-settings")]
        public async Task<IActionResult> GetDetailCockFightPlayerBetSettings([FromRoute] long playerId)
        {
            var result = await _cockFightPlayerBetSettingService.GetCockFightPlayerBetSettingDetail(playerId);
            return Ok(OkResponse.Create(result));
        }

        [HttpPut("{playerId:long}/bet-settings")]
        public async Task<IActionResult> UpdatePlayerBetSetting([FromRoute] long playerId, [FromBody] UpdateCockFightPlayerBetSettingRequest request)
        {
            await _cockFightPlayerBetSettingService.UpdateCockFightPlayerBetSetting(playerId, new UpdateCockFightAgentBetSettingModel
            {
                BetKindId = request.BetKindId,
                MainLimitAmountPerFight = request.MainLimitAmountPerFight,
                DrawLimitAmountPerFight = request.DrawLimitAmountPerFight,
                LimitNumTicketPerFight = request.LimitNumTicketPerFight
            });
            return Ok();
        }
    }
}