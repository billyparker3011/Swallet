using HnMicro.Framework.Controllers;
using HnMicro.Framework.Responses;
using Lottery.Core.Partners.Models.Bti;
using Lottery.Core.Services.Partners.Bti;
using Microsoft.AspNetCore.Mvc;

namespace Lottery.Agent.AgentService.Controllers
{
    public class BtiAgentPositionTakingController : HnControllerBase
    {
        private readonly IBtiAgentPositionTakingService _btiAgentPositionTakingService;
        public BtiAgentPositionTakingController(IBtiAgentPositionTakingService btiAgentPositionTakingService)
        {
            _btiAgentPositionTakingService = btiAgentPositionTakingService;
        }

        [HttpGet("agent-positions/{agentId}")]
        public async Task<IActionResult> Gets([FromRoute] long agentId)
        {
            if (agentId < 1) return NotFound();
            return Ok(OkResponse.Create(await _btiAgentPositionTakingService.GetsAsync(agentId)));
        }

        [HttpGet("agent-positions")]
        public async Task<IActionResult> GetAll()
        {
            return Ok(OkResponse.Create(await _btiAgentPositionTakingService.GetAllAsync()));
        }

        [HttpGet("agent-position/{id:int}")]
        public async Task<IActionResult> Find([FromRoute] int id)
        {
            if (id < 1) return NotFound();
            return Ok(OkResponse.Create(await _btiAgentPositionTakingService.FindAsync(id)));
        }

        [HttpPost("agent-position")]
        public async Task<IActionResult> Create(BtiAgentPositionTakingModel model)
        {
            await _btiAgentPositionTakingService.CreateAsync(model);
            return Ok();
        }

        [HttpPost("agent-positions")]
        public async Task<IActionResult> Creates(List<BtiAgentPositionTakingModel> models)
        {
            foreach (var model in models)
            {
                model.Id = 0;
                await _btiAgentPositionTakingService.CreateAsync(model);
            }
            return Ok();
        }

        [HttpPut("{id:int}/agent-position")]
        public async Task<IActionResult> Update([FromRoute] int id, [FromBody] BtiAgentPositionTakingModel model)
        {
            if (id < 1) return NotFound();
            model.Id = id;
            await _btiAgentPositionTakingService.UpdateAsync(model);
            return Ok();
        }

        [HttpPut("agent-positions")]
        public async Task<IActionResult> Updates(List<BtiAgentPositionTakingModel> models)
        {
            foreach (var model in models)
            {
                if (model.Id < 1) return NotFound(model.Id);
                await _btiAgentPositionTakingService.UpdateAsync(model);
            }
            return Ok();
        }

        [HttpDelete("{id:int}/agent-position")]
        public async Task<IActionResult> Delete([FromRoute] int id)
        {
            if (id < 1) return NotFound();
            await _btiAgentPositionTakingService.DeleteAsync(id);
            return Ok();
        }
    }
}
