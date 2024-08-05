using HnMicro.Framework.Controllers;
using Lottery.Core.Services.Audit;
using Microsoft.AspNetCore.Mvc;

namespace Lottery.Player.PlayerService.Controllers
{
    public class CockFightPlayerController : HnControllerBase
    {
        private readonly ICockFightService _cockFightService;

        public CockFightPlayerController(ICockFightService cockFightService)
        {
            _cockFightService = cockFightService;
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
    }
}