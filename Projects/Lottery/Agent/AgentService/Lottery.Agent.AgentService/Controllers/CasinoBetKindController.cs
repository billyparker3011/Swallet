using HnMicro.Framework.Controllers;
using HnMicro.Framework.Responses;
using Lottery.Core.Partners.Models.Allbet;
using Lottery.Core.Services.Partners.CA;
using Microsoft.AspNetCore.Mvc;

namespace Lottery.Agent.AgentService.Controllers
{
    public class CasinoBetKindController : HnControllerBase
    {
        private readonly ICasinoBetKindService _cABetKindService;
        public CasinoBetKindController(ICasinoBetKindService cABetKindService)
        {
            _cABetKindService = cABetKindService;
        }

        [HttpGet("bet-kinds/{name}")]
        public async Task<IActionResult> GetBetKinds([FromRoute] string name)
        {
            if (string.IsNullOrWhiteSpace(name)) return NotFound();
            return Ok(OkResponse.Create(await _cABetKindService.GetBetKindsAsync(name)));
        }

        [HttpGet("bet-kinds")]
        public async Task<IActionResult> GetAllBetKinds()
        {
            return Ok(OkResponse.Create(await _cABetKindService.GetAllBetKindsAsync()));
        }

        [HttpGet("bet-kind/{id:int}")]
        public async Task<IActionResult> FindBetKind([FromRoute] int id)
        {
            if (id < 1) return NotFound();
            return Ok(OkResponse.Create(await _cABetKindService.FindBetKindAsync(id)));
        }

        [HttpPost("bet-kind")]
        public async Task<IActionResult> CreateBetKind(CreateCasinoBetKindModel model)
        {
            await _cABetKindService.CreateBetKindAsync(model);
            return Ok();
        }

        [HttpPut("{id:int}/bet-kind")]
        public async Task<IActionResult> UpdateBetKind([FromRoute] int id, [FromBody] UpdateCasinoBetKindModel model)
        {
            if (id < 1) return NotFound();
            model.Id = id;
            await _cABetKindService.UpdateBetKindAsync(model);
            return Ok();
        }

        [HttpDelete("{id:int}/bet-kind")]
        public async Task<IActionResult> DeleteBetKind([FromRoute] int id)
        {
            if (id < 1) return NotFound();
            await _cABetKindService.DeleteBetKindAsync(id);
            return Ok();
        }
    }
}
