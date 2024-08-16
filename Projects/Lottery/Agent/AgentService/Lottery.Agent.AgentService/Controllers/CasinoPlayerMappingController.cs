using HnMicro.Framework.Controllers;
using HnMicro.Framework.Responses;
using Lottery.Core.Services.Partners.CA;
using Microsoft.AspNetCore.Mvc;

namespace Lottery.Agent.AgentService.Controllers
{
    public class CasinoPlayerMappingController : HnControllerBase
    {
        private readonly ICasinoPlayerMappingService _cAPlayerMappingService;
        public CasinoPlayerMappingController(ICasinoPlayerMappingService cAPlayerMappingService)
        {
            _cAPlayerMappingService = cAPlayerMappingService;
        }

        [HttpGet("player-mapping/{playerId:long}")]
        public async Task<IActionResult> FindPlayerMappingByPlayerId([FromRoute] long playerId)
        {
            if (playerId < 1) return NotFound();
            return Ok(OkResponse.Create(await _cAPlayerMappingService.FindPlayerMappingByPlayerIdAsync(playerId)));
        }
    }
}
