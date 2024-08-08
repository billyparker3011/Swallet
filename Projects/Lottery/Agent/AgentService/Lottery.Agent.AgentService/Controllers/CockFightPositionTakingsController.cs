using HnMicro.Framework.Controllers;
using HnMicro.Framework.Responses;
using Lottery.Agent.AgentService.Requests.Agent;
using Lottery.Agent.AgentService.Requests.CockFight;
using Lottery.Core.Models.CockFight.UpdateCockFightAgentPositionTaking;
using Lottery.Core.Services.CockFight;
using Microsoft.AspNetCore.Mvc;

namespace Lottery.Agent.AgentService.Controllers
{
    public class CockFightPositionTakingsController : HnControllerBase
    {
        private readonly ICockFightAgentPositionTakingService _cockFightAgentPositionTakingService;

        public CockFightPositionTakingsController(ICockFightAgentPositionTakingService cockFightAgentPositionTakingService)
        {
            _cockFightAgentPositionTakingService = cockFightAgentPositionTakingService;
        }

        [HttpGet("{agentId:long}/position-takings")]
        public async Task<IActionResult> GetDetailCockFightAgentPositionTakings([FromRoute] long agentId)
        {
            var result = await _cockFightAgentPositionTakingService.GetCockFightAgentPositionTakingDetail(agentId);
            return Ok(OkResponse.Create(result));
        }

        [HttpPut("{agentId:long}/position-taking")]
        public async Task<IActionResult> UpdateCockFightAgentPositionTaking([FromRoute] long agentId, [FromBody] UpdateCockFightAgentPositionTakingRequest request)
        {
            await _cockFightAgentPositionTakingService.UpdateCockFightAgentPositionTaking(agentId, new UpdateCockFightAgentPositionTakingModel
            {
                BetKindId = request.BetKindId,
                ActualPositionTaking = request.ActualPositionTaking
            });
            return Ok();
        }
    }
}
