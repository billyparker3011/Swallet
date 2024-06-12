using HnMicro.Framework.Controllers;
using HnMicro.Framework.Responses;
using Lottery.Core.Services.Player;
using Microsoft.AspNetCore.Mvc;

namespace Lottery.Player.PlayerService.Controllers
{
    public class NavigationController : HnControllerBase
    {
        private readonly IPlayerNavigationService _playerNavigationService;

        public NavigationController(IPlayerNavigationService playerNavigationService)
        {
            _playerNavigationService = playerNavigationService;
        }

        [HttpGet]
        public async Task<IActionResult> GetNavigation()
        {
            return Ok(OkResponse.Create(await _playerNavigationService.MyNavigation()));
        }
    }
}