using HnMicro.Framework.Controllers;
using HnMicro.Framework.Responses;
using Lottery.Core.Services.Authentication;
using Microsoft.AspNetCore.Mvc;

namespace Lottery.Player.PlayerService.Controllers
{
    public class SessionController : HnControllerBase
    {
        private readonly ISessionService _sessionService;

        public SessionController(ISessionService sessionService)
        {
            _sessionService = sessionService;
        }

        [HttpGet]
        public async Task<IActionResult> RecheckIn()
        {
            return Ok(OkResponse.Create(await _sessionService.RecheckIn(false)));
        }
    }
}