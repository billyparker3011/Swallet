using HnMicro.Framework.Controllers;
using HnMicro.Framework.Responses;
using Lottery.Core.Partners.Models.Allbet;
using Lottery.Core.Services.Partners.CA;
using Microsoft.AspNetCore.Mvc;

namespace Lottery.Agent.AgentService.Controllers
{
    public class CasinoAgentPositionTakingController : HnControllerBase
    {
        private readonly ICasinoAgentPositionTakingService _cAAgentPositionTakingService;
        public CasinoAgentPositionTakingController(ICasinoAgentPositionTakingService cAAgentPositionTakingService)
        {
            _cAAgentPositionTakingService = cAAgentPositionTakingService;
        }

        [HttpGet("agent-positions/{agentId:long}")]
        public async Task<IActionResult> GetAgentPositionTakings([FromRoute] long agentId)
        {
            if (agentId < 1) return NotFound();
            return Ok(OkResponse.Create(await _cAAgentPositionTakingService.GetAgentPositionTakingsAsync(agentId)));
        }

        [HttpGet("agent-positions")]
        public async Task<IActionResult> GetAllAgentPositionTakings()
        {
            return Ok(OkResponse.Create(await _cAAgentPositionTakingService.GetAllAgentPositionTakingsAsync()));
        }

        [HttpGet("agent-position/{id:long}")]
        public async Task<IActionResult> GetAgentPositionTaking(long id)
        {
            if (id < 1) return NotFound();
            return Ok(OkResponse.Create(await _cAAgentPositionTakingService.FindAgentPositionTakingAsync(id)));
        }

        [HttpPost("agent-position")]
        public async Task<IActionResult> CreateAgentPositionTaking(CreateCasinoAgentPositionTakingModel model)
        {
            await _cAAgentPositionTakingService.CreateAgentPositionTakingAsync(model);
            return Ok();
        }
        
        [HttpPost("agent-positions")]
        public async Task<IActionResult> CreateAgentPositionTakings(List<CreateCasinoAgentPositionTakingModel> models)
        {
            foreach (var model in models)
            {
                await _cAAgentPositionTakingService.CreateAgentPositionTakingAsync(model);
            }

            return Ok();
        }

        [HttpPut("{id:long}/agent-position")]
        public async Task<IActionResult> UpdateAgentPositionTaking([FromRoute] long id, [FromBody] UpdateCasinoAgentPositionTakingModel model)
        {
            if (id < 1) return NotFound();
            model.Id = id;
            await _cAAgentPositionTakingService.UpdateAgentPositionTakingAsync(model);
            return Ok();
        }

        [HttpPut("agent-positions")]
        public async Task<IActionResult> UpdateAgentPositionTakings(List<UpdateCasinoAgentPositionTakingModel> models)
        {
            foreach (var model in models)
            {
                if (model.Id < 1) return NotFound(model.Id);
                await _cAAgentPositionTakingService.UpdateAgentPositionTakingAsync(model);
            }

            return Ok();
        }

        [HttpDelete("{id:long}/agent-position")]
        public async Task<IActionResult> DeleteAgentPositionTaking([FromRoute] long id)
        {
            if (id < 1) return NotFound();
            await _cAAgentPositionTakingService.DeleteAgentPositionTakingAsync(id);
            return Ok();
        }
    }
}
