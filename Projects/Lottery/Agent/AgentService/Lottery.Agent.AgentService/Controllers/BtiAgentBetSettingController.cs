using HnMicro.Framework.Controllers;
using HnMicro.Framework.Responses;
using Lottery.Core.Partners.Models.Bti;
using Lottery.Core.Services.Partners.Bti;
using Microsoft.AspNetCore.Mvc;

namespace Lottery.Agent.AgentService.Controllers
{
    public class BtiAgentBetSettingController : HnControllerBase
    {
        private readonly IBtiAgentBetSettingService _btiAgentBetSettingService;
        public BtiAgentBetSettingController(IBtiAgentBetSettingService btiAgentBetSettingService)
        {
            _btiAgentBetSettingService = btiAgentBetSettingService;
        }

        [HttpGet("bet-settings/{agent}")]
        public async Task<IActionResult> Gets([FromRoute] long agentId)
        {
            if (agentId < 1) return NotFound();
            return Ok(OkResponse.Create(await _btiAgentBetSettingService.GetsAsync(agentId)));
        }

        [HttpGet("bet-settings")]
        public async Task<IActionResult> GetAll()
        {
            return Ok(OkResponse.Create(await _btiAgentBetSettingService.GetAllAsync()));
        }

        [HttpGet("bet-setting/{id:int}")]
        public async Task<IActionResult> Find([FromRoute] int id)
        {
            if (id < 1) return NotFound();
            return Ok(OkResponse.Create(await _btiAgentBetSettingService.FindAsync(id)));
        }

        [HttpPost("bet-setting")]
        public async Task<IActionResult> Create(BtiAgentBetSettingModel model)
        {
            await _btiAgentBetSettingService.CreateAsync(model);
            return Ok();
        }

        [HttpPost("bet-settings")]
        public async Task<IActionResult> Creates(List<BtiAgentBetSettingModel> models)
        {
            foreach (var model in models)
            {
                model.Id = 0;
                await _btiAgentBetSettingService.CreateAsync(model);
            }
            return Ok();
        }

        [HttpPut("{id:int}/bet-setting")]
        public async Task<IActionResult> Update([FromRoute] int id, [FromBody] BtiAgentBetSettingModel model)
        {
            if (id < 1) return NotFound();
            model.Id = id;
            await _btiAgentBetSettingService.UpdateAsync(model);
            return Ok();
        }

        [HttpPut("bet-settings")]
        public async Task<IActionResult> Updates(List<BtiAgentBetSettingModel> models)
        {
            foreach (var model in models)
            {
                if (model.Id < 1) return NotFound(model.Id);
                await _btiAgentBetSettingService.UpdateAsync(model);
            }
            return Ok();
        }

        [HttpDelete("{id:int}/bet-setting")]
        public async Task<IActionResult> Delete([FromRoute] int id)
        {
            if (id < 1) return NotFound();
            await _btiAgentBetSettingService.DeleteAsync(id);
            return Ok();
        }
    }
}
