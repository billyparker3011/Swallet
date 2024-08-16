using HnMicro.Framework.Controllers;
using HnMicro.Framework.Responses;
using Lottery.Core.Services.Partners.CA;
using Microsoft.AspNetCore.Mvc;

namespace Lottery.Agent.AgentService.Controllers
{
    public class CasinoAgentHandicapController : HnControllerBase
    {
        private readonly ICasinoAgentHandicapService _cAAgentHandicapService;
        public CasinoAgentHandicapController(ICasinoAgentHandicapService cAAgentHandicapService)
        {
            _cAAgentHandicapService = cAAgentHandicapService;
        }

        [HttpGet("agent-handicaps/{handicapType}")]
        public async Task<IActionResult> GetAgentHandicaps([FromRoute] string handicapType)
        {
            if (string.IsNullOrWhiteSpace(handicapType)) return NotFound();
            return Ok(OkResponse.Create(await _cAAgentHandicapService.GetAgentHandicapsAsync(handicapType)));
        }

        [HttpGet("agent-handicaps")]
        public async Task<IActionResult> GetAllAgentHandicaps()
        {
            return Ok(OkResponse.Create(await _cAAgentHandicapService.GetAllAgentHandicapsAsync()));
        }       
    }
}
