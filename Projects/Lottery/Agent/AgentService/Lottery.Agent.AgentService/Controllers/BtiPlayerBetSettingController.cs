using HnMicro.Framework.Controllers;
using HnMicro.Framework.Responses;
using Lottery.Core.Partners.Models.Bti;
using Lottery.Core.Services.Partners.Bti;
using Microsoft.AspNetCore.Mvc;

namespace Lottery.Agent.AgentService.Controllers
{
    public class BtiPlayerBetSettingController : HnControllerBase
    {
        private readonly IBtiPlayerBetSettingService _btiPlayerBetSettingService;
        public BtiPlayerBetSettingController(IBtiPlayerBetSettingService btiPlayerBetSettingService)
        {
            _btiPlayerBetSettingService = btiPlayerBetSettingService;
        }

        [HttpGet("bet-settings/{agent}")]
        public async Task<IActionResult> Gets([FromRoute] long agentId)
        {
            if (agentId < 1) return NotFound();
            return Ok(OkResponse.Create(await _btiPlayerBetSettingService.GetsAsync(agentId)));
        }

        [HttpGet("bet-settings")]
        public async Task<IActionResult> GetAll()
        {
            return Ok(OkResponse.Create(await _btiPlayerBetSettingService.GetAllAsync()));
        }

        [HttpGet("bet-setting/{id:int}")]
        public async Task<IActionResult> Find([FromRoute] int id)
        {
            if (id < 1) return NotFound();
            return Ok(OkResponse.Create(await _btiPlayerBetSettingService.FindAsync(id)));
        }

        [HttpPost("bet-setting")]
        public async Task<IActionResult> Create(BtiPlayerBetSettingModel model)
        {
            await _btiPlayerBetSettingService.CreateAsync(model);
            return Ok();
        }

        [HttpPost("bet-settings")]
        public async Task<IActionResult> Creates(List<BtiPlayerBetSettingModel> models)
        {
            foreach (var model in models)
            {
                model.Id = 0;
                await _btiPlayerBetSettingService.CreateAsync(model);
            }
            return Ok();
        }

        [HttpPut("{id:int}/bet-setting")]
        public async Task<IActionResult> Update([FromRoute] int id, [FromBody] BtiPlayerBetSettingModel model)
        {
            if (id < 1) return NotFound();
            model.Id = id;
            await _btiPlayerBetSettingService.UpdateAsync(model);
            return Ok();
        }

        [HttpPut("bet-settings")]
        public async Task<IActionResult> Updates(List<BtiPlayerBetSettingModel> models)
        {
            foreach (var model in models)
            {
                if (model.Id < 1) return NotFound(model.Id);
                await _btiPlayerBetSettingService.UpdateAsync(model);
            }
            return Ok();
        }

        [HttpDelete("{id:int}/bet-setting")]
        public async Task<IActionResult> Delete([FromRoute] int id)
        {
            if (id < 1) return NotFound();
            await _btiPlayerBetSettingService.DeleteAsync(id);
            return Ok();
        }
    }
}
