using HnMicro.Framework.Controllers;
using HnMicro.Framework.Responses;
using Lottery.Core.Enums;
using Lottery.Core.Services.Partners.CA;
using Microsoft.AspNetCore.Mvc;

namespace Lottery.Agent.AgentService.Controllers
{
    public class CasinoGameTypeController : HnControllerBase
    {
        private readonly ICasinoGameTypeService _cAGameTypeService;
        public CasinoGameTypeController(ICasinoGameTypeService cAGameTypeService)
        {
            _cAGameTypeService = cAGameTypeService;
        }

        [HttpGet("game-types/{category}")]
        public async Task<IActionResult> GetGameTypes([FromRoute] string category)
        {

            if (string.IsNullOrWhiteSpace(category)) return NotFound();
            return Ok(OkResponse.Create(await _cAGameTypeService.GetGameTypesAsync(category)));    
        }

        [HttpGet("game-types")]
        public async Task<IActionResult> GetAllGameTypes()
        {
            return Ok(OkResponse.Create(await _cAGameTypeService.GetAllGameTypesAsync()));
        }


    }
}
