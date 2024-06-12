using HnMicro.Framework.Controllers;
using Lottery.Core.Services.Player;
using Lottery.Player.PlayerService.Requests.Safeguard;
using Microsoft.AspNetCore.Mvc;

namespace Lottery.Player.PlayerService.Controllers
{
    public class SafeguardController : HnControllerBase
    {
        private readonly IPlayerSafeguardService _playerSafeguardService;

        public SafeguardController(IPlayerSafeguardService playerSafeguardService)
        {
            _playerSafeguardService = playerSafeguardService;
        }

        [HttpPost("change-password")]
        public async Task<IActionResult> ChangePassword(ChangePasswordRequest request)
        {
            await _playerSafeguardService.ChangePassword(new Core.Models.Auth.PlayerChangePasswordModel { OldPassword = request.OldPassword, NewPassword = request.NewPassword, ConfirmPassword = request.ConfirmPassword });
            return Ok();
        }

        [HttpPut("{playerId:long}/reset-password")]
        public async Task<IActionResult> ResetPassword([FromRoute] long playerId, ResetPasswordRequest request)
        {
            await _playerSafeguardService.ResetPassword(new Core.Models.Auth.ResetPasswordModel { TargetId = playerId, Password = request.Password, ConfirmPassword = request.ConfirmPassword });
            return Ok();
        }
    }
}