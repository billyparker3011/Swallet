using HnMicro.Framework.Controllers;
using HnMicro.Framework.Responses;
using Lottery.Core.Models.CockFight.GetCockFightAgentBetKind;
using Lottery.Core.Services.CockFight;
using Microsoft.AspNetCore.Mvc;

namespace Lottery.Agent.AgentService.Controllers
{
    public class CockFightBetKindController : HnControllerBase
    {
        private readonly ICockFightAgentBetKindService _cockFightAgentBetKindService;

        public CockFightBetKindController(ICockFightAgentBetKindService cockFightAgentBetKindService)
        {
            _cockFightAgentBetKindService = cockFightAgentBetKindService;
        }

        [HttpGet("bet-kind")]
        public async Task<IActionResult> GetMasterCockFightBetKind()
        {
            var result = await _cockFightAgentBetKindService.GetCockFightAgentBetKind();
            return Ok(OkResponse.Create(result));
        }

        [HttpPut("bet-kind")]
        public async Task<IActionResult> UpdateMasterCockFightBetKind([FromBody] GetCockFightAgentBetKindModel request)
        {
            await _cockFightAgentBetKindService.UpdateCockFightAgentBetKind(new GetCockFightAgentBetKindModel
            {
                Name = request.Name,
                Enabled = request.Enabled
            });
            return Ok();
        }
    }
}
