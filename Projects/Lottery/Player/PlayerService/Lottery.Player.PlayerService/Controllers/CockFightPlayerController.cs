using HnMicro.Framework.Controllers;
using HnMicro.Framework.Responses;
using Lottery.Core.Models.CockFight.UpdateCockFightAgentBetSetting;
using Lottery.Core.Partners.Attribute.CockFight;
using Lottery.Core.Services.CockFight;
using Lottery.Player.PlayerService.Requests.CockFight;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Lottery.Player.PlayerService.Controllers
{
    public class CockFightPlayerController : HnControllerBase
    {
        private readonly ICockFightService _cockFightService;
        private readonly ICockFightPlayerBetSettingService _cockFightPlayerBetSettingService;

        public CockFightPlayerController(ICockFightService cockFightService, ICockFightPlayerBetSettingService cockFightPlayerBetSettingService)
        {
            _cockFightService = cockFightService;
            _cockFightPlayerBetSettingService = cockFightPlayerBetSettingService;
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
        public async Task<IActionResult> GetCockFightPlayerBalance()
        {
            return Ok(await _cockFightService.GetCockFightPlayerBalance());
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