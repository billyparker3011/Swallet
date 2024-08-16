using HnMicro.Framework.Controllers;
using HnMicro.Framework.Responses;
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

        [HttpGet("game-types/{caterory}")]
        public async Task<IActionResult> GetGameTypes([FromRoute] string caterory)
        {

            if (string.IsNullOrWhiteSpace(caterory)) return NotFound();
            return Ok(OkResponse.Create(await _cAGameTypeService.GetGameTypesAsync(caterory)));    
        }

        [HttpGet("game-types")]
        public async Task<IActionResult> GetAllGameTypes()
        {
            return Ok(OkResponse.Create(await _cAGameTypeService.GetAllGameTypesAsync()));
        }


    }
}
