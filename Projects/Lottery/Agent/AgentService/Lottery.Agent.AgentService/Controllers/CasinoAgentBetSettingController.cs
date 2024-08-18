using HnMicro.Framework.Controllers;
using HnMicro.Framework.Responses;
using Lottery.Core.Partners.Models.Allbet;
using Lottery.Core.Services.Partners.CA;
using Microsoft.AspNetCore.Mvc;

namespace Lottery.Agent.AgentService.Controllers
{
    public class CasinoAgentBetSettingController : HnControllerBase
    {
        private readonly ICasinoAgentBetSettingService _cAAgentBetSettingService;
        public CasinoAgentBetSettingController(ICasinoAgentBetSettingService cAAgentBetSettingService)
        {
            _cAAgentBetSettingService = cAAgentBetSettingService;
        }

        [HttpGet("bet-settings/{agentId:long}")]
        public async Task<IActionResult> GetAgentBetSettings([FromRoute] long agentId)
        {            
            if (agentId < 1) return NotFound();
            return Ok(OkResponse.Create(await _cAAgentBetSettingService.GetAgentBetSettingsAsync(agentId)));
        }

        [HttpGet("bet-settings")]
        public async Task<IActionResult> GetAllAgentBetSettings()
        {
            return Ok(OkResponse.Create(await _cAAgentBetSettingService.GetAllAgentBetSettingsAsync()));
        }

        [HttpGet("bet-setting/{id:long}")]
        public async Task<IActionResult> FindAgentBetSetting(long id)
        {
            if(id < 1) return NotFound();
            return Ok(OkResponse.Create(await _cAAgentBetSettingService.FindAgentBetSettingAsync(id)));
        }

        [HttpPost("bet-setting")]
        public async Task<IActionResult> CreateAgentBetSetting(CreateCasinoAgentBetSettingModel model)
        {

            await _cAAgentBetSettingService.CreateAgentBetSettingAsync(model);
            return Ok();

        }

        [HttpPost("bet-settings")]
        public async Task<IActionResult> CreateAgentBetSettings(List<CreateCasinoAgentBetSettingModel> models)
        {

            foreach (var model in models)
            {
                await _cAAgentBetSettingService.CreateAgentBetSettingAsync(model);
            }

            return Ok();
        }

        [HttpPut("{id:long}/bet-setting")]
        public async Task<IActionResult> UpdateAgentBetSetting([FromRoute] long id, [FromBody] UpdateCasinoAgentBetSettingModel model)
        {
            if (id < 1) return NotFound();
            model.Id = id;
            await _cAAgentBetSettingService.UpdateAgentBetSettingAsync(model);
            return Ok();
        }

        [HttpPut("bet-settings")]
        public async Task<IActionResult> UpdateAgentBetSettings(List<UpdateCasinoAgentBetSettingModel> models)
        {
            foreach (var model in models)
            {
                if (model.Id < 1) return NotFound(model.Id);
                await _cAAgentBetSettingService.UpdateAgentBetSettingAsync(model);             
            }
            return Ok();
        }

        [HttpDelete("{id:long}/bet-setting")]
        public async Task<IActionResult> DeleteAgentBetSetting([FromRoute] long id)
        {
            if (id < 1) return NotFound();
            await _cAAgentBetSettingService.DeleteAgentBetSettingAsync(id);
            return Ok();
        }
    }
}
