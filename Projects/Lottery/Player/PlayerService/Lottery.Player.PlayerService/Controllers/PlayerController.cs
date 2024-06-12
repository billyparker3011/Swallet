using HnMicro.Framework.Controllers;
using HnMicro.Framework.Responses;
using Lottery.Core.Models.Player.CreatePlayer;
using Lottery.Core.Models.Player.UpdatePlayer;
using Lottery.Core.Models.Player.UpdatePlayerCreditBalance;
using Lottery.Core.Services.Player;
using Lottery.Player.PlayerService.Requests.Player;
using Microsoft.AspNetCore.Mvc;

namespace Lottery.Player.PlayerService.Controllers
{
    public class PlayerController : HnControllerBase
    {
        private readonly IPlayerService _playerService;
        private readonly IPlayerCreditService _playerCreditService;
        private readonly IPlayerSettingService _playerSettingService;

        public PlayerController(IPlayerService playerService, IPlayerCreditService playerCreditService, IPlayerSettingService playerSettingService)
        {
            _playerService = playerService;
            _playerCreditService = playerCreditService;
            _playerSettingService = playerSettingService;
        }

        [HttpGet("{playerId:long}/bet-settings")]
        public async Task<IActionResult> GetDetailPlayerBetSettings([FromRoute] long playerId)
        {
            var result = await _playerService.GetDetailPlayerBetSettings(playerId);
            return Ok(OkResponse.Create(result.AgentBetSettings));
        }

        [HttpPut("{playerId:long}/bet-setting")]
        public async Task<IActionResult> UpdatePlayerBetSetting([FromRoute] long playerId, [FromBody] UpdatePlayerBetSettingRequest request)
        {
            await _playerService.UpdatePlayerBetSetting(playerId, request.BetSettings);
            return Ok();
        }

        [HttpPut("{playerId:long}")]
        public async Task<IActionResult> UpdatePlayer([FromRoute] long playerId, [FromBody] UpdatePlayerRequest request)
        {
            await _playerService.UpdatePlayer(new UpdatePlayerModel
            {
                PlayerId = playerId,
                State = request.State,
                FirstName = request.FirstName,
                LastName = request.LastName,
                Credit = request.Credit
            });
            return Ok();
        }

        [HttpGet("logout")]
        public async Task<IActionResult> Logout()
        {
            await _playerService.Logout();
            return Ok();
        }

        [HttpGet("my-balance")]
        public async Task<IActionResult> MyBalance()
        {
            return Ok(OkResponse.Create(await _playerCreditService.GetMyBalance()));
        }

        [HttpGet("my-bet-setting/{betKindId:int}")]
        public async Task<IActionResult> MyBetSettingByBetKind([FromRoute] int betKindId)
        {
            return Ok(OkResponse.Create(await _playerSettingService.GetMyBetSettingByBetKind(betKindId)));
        }

        [HttpGet("my-bet-setting/mixed/{betKindId:int}")]
        public async Task<IActionResult> MyMixedBetSettingByBetKind([FromRoute] int betKindId)
        {
            return Ok(OkResponse.Create(await _playerSettingService.GetMyMixedBetSettingByBetKind(betKindId)));
        }

        [HttpPost]
        public async Task<IActionResult> CreatePlayer([FromBody] CreatePlayerRequest request)
        {
            await _playerService.CreatePlayer(new CreatePlayerModel
            {
                Username = request.Username,
                Password = request.Password,
                Credit = request.Credit,
                FirstName = request.FirstName,
                LastName = request.LastName,
                MemberMaxCredit = request.MemberMaxCredit,
                BetSettings = request.BetSettings
            });
            return Ok();
        }

        [HttpGet("suggestion-identifier")]
        public async Task<IActionResult> GetSuggestionPlayerIdentifier()
        {
            return Ok(OkResponse.Create(await _playerService.GetSuggestionPlayerIdentifier()));
        }

        [HttpGet("{playerId:long}/credit-balance")]
        public async Task<IActionResult> GetCreditBalanceDetailPopup([FromRoute] long playerId)
        {
            var result = await _playerService.GetCreditBalanceDetailPopup(playerId);
            return Ok(OkResponse.Create(result));
        }

        [HttpPut("{playerId:long}/credit-balance")]
        public async Task<IActionResult> ModifyPlayerCreditBalance([FromRoute] long playerId, [FromBody] UpdatePlayerCreditBalanceRequest request)
        {
            await _playerService.UpdatePlayerCreditBalance(new UpdatePlayerCreditBalanceModel
            {
                PlayerId = playerId,
                Credit = request.Credit
            });
            return Ok();
        }
    }
}