using HnMicro.Framework.Controllers;
using HnMicro.Framework.Responses;
using Lottery.Core.Partners.Models.Bti;
using Lottery.Core.Services.Partners.Bti;
using Microsoft.AspNetCore.Mvc;

namespace Lottery.Agent.AgentService.Controllers
{
    public class BtiBetKindController : HnControllerBase
    {
        private readonly IBtiBetKindService _btiBetKindService;
        public BtiBetKindController(IBtiBetKindService btiBetKindService)
        {
            _btiBetKindService = btiBetKindService;
        }

        [HttpGet("bet-kinds/{name}")]
        public async Task<IActionResult> Gets([FromRoute] string name)
        {
            if (string.IsNullOrWhiteSpace(name)) return NotFound();
            return Ok(OkResponse.Create(await _btiBetKindService.GetsAsync(name)));
        }

        [HttpGet("bet-kinds")]
        public async Task<IActionResult> GetAll()
        {
            return Ok(OkResponse.Create(await _btiBetKindService.GetAllAsync()));
        }

        [HttpGet("bet-kind/{id:int}")]
        public async Task<IActionResult> Find([FromRoute] int id)
        {
            if (id < 1) return NotFound();
            return Ok(OkResponse.Create(await _btiBetKindService.FindAsync(id)));
        }

        [HttpPost("bet-kind")]
        public async Task<IActionResult> Create(BtiBetKindModel model)
        {
            await _btiBetKindService.CreateAsync(model);
            return Ok();
        }

        [HttpPost("bet-kinds")]
        public async Task<IActionResult> Creates(List<BtiBetKindModel> models)
        {          
            foreach (var model in models)
            {
                model.Id = 0;
                await _btiBetKindService.CreateAsync(model);
            }
            return Ok();
        }

        [HttpPut("{id:int}/bet-kind")]
        public async Task<IActionResult> Update([FromRoute] int id, [FromBody] BtiBetKindModel model)
        {
            if (id < 1) return NotFound();
            model.Id = id;
            await _btiBetKindService.UpdateAsync(model);
            return Ok();
        }

        [HttpPut("bet-kinds")]
        public async Task<IActionResult> Updates(List<BtiBetKindModel> models)
        {
            foreach (var model in models)
            {
                if (model.Id < 1) return NotFound(model.Id);
                await _btiBetKindService.UpdateAsync(model);
            }
            return Ok();
        }

        [HttpDelete("{id:int}/bet-kind")]
        public async Task<IActionResult> Delete([FromRoute] int id)
        {
            if (id < 1) return NotFound();
            await _btiBetKindService.DeleteAsync(id);
            return Ok();
        }
    }
}
