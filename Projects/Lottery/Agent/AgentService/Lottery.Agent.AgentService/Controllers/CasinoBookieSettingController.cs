using HnMicro.Framework.Controllers;
using HnMicro.Framework.Responses;
using Lottery.Core.Partners.Models.Allbet;
using Lottery.Core.Services.Partners.CA;
using Microsoft.AspNetCore.Mvc;

namespace Lottery.Agent.AgentService.Controllers
{
    public class CasinoBookieSettingController : HnControllerBase
    {
        private readonly ICasinoBookieSettingService _casinoBookieSettingService;
        public CasinoBookieSettingController(ICasinoBookieSettingService casinoBookieSettingService)
        {
            _casinoBookieSettingService = casinoBookieSettingService;
        }

        [HttpGet("bookie-setting")]
        public async Task<IActionResult> Get()
        {         
            return Ok(OkResponse.Create(await _casinoBookieSettingService.GetCasinoBookieSettingValueAsync()));
        }

        [HttpPost("bookie-setting")]
        public async Task<IActionResult> Post(AllbetBookieSettingValue model)
        {
            await _casinoBookieSettingService.CreateCasinoBookieSettingValueAsync(model);
            return Ok();
        }

        [HttpPut("{id:int}/bookie-setting")]
        public async Task<IActionResult> Put([FromRoute] int id, [FromBody] AllbetBookieSettingValue model)
        {
            if (id < 1) return NotFound();
            await _casinoBookieSettingService.UpdateCasinoBookieSettingValueAsync(id, model);
            return Ok();
        }
    }
}
