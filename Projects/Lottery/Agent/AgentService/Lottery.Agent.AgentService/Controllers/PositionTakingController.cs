using HnMicro.Framework.Controllers;
using HnMicro.Framework.Responses;
using Lottery.Agent.AgentService.Requests.Agent;
using Lottery.Core.Models.Agent.GetAgentPositionTaking;
using Lottery.Core.Services.Agent;
using Microsoft.AspNetCore.Mvc;

namespace Lottery.Agent.AgentService.Controllers
{
    [Route(Core.Helpers.RouteHelper.V1.PositionTaking.BaseRoute)]
    public class PositionTakingController : HnControllerBase
    {
        private readonly IAgentPositionTakingService _agentPositionTakingService;

        public PositionTakingController(IAgentPositionTakingService agentPositionTakingService)
        {
            _agentPositionTakingService = agentPositionTakingService;
        }

        [HttpGet]
        public async Task<ActionResult<ApiResponse<GetAgentPositionTakingResult>>> GetAgentPositionTakings()
        {
            var result = await _agentPositionTakingService.GetAgentPositionTakings();
            return Ok(OkResponse.Create(result.AgentPositionTakings));
        }

        [HttpPut]
        public async Task<IActionResult> ModifyAgentPositionTakings([FromBody] UpdateAgentPositionTakingRequest request)
        {
            await _agentPositionTakingService.UpdateAgentPositionTakings(request.PositionTakings);
            return Ok();
        }
    }
}
