using HnMicro.Framework.Controllers;
using HnMicro.Framework.Enums;
using HnMicro.Framework.Models;
using HnMicro.Framework.Responses;
using Lottery.Core.Models.Agent.GetAgentOuts;
using Lottery.Core.Models.Agent.GetAgentOutstanding;
using Lottery.Core.Services.Agent;
using Microsoft.AspNetCore.Mvc;

namespace Lottery.Agent.AgentService.Controllers
{
    public class OutstandingController : HnControllerBase
    {
        private readonly IAgentOutstandingService _agentOutstandingService;

        public OutstandingController(IAgentOutstandingService agentOutstandingService)
        {
            _agentOutstandingService = agentOutstandingService;
        }

        [HttpGet]
        public async Task<ActionResult<ApiResponse<GetAgentOutstandingResult>>> GetAgentPositionTakings([FromQuery] long? agentId, [FromQuery] int? roleId, [FromQuery] string sortName, [FromQuery] SortType sortType = SortType.Descending)
        {
            var result = await _agentOutstandingService.GetAgentOutstandings(new GetAgentOutstandingModel
            {
                AgentId = agentId,
                RoleId = roleId,
                SortName = sortName,
                SortType = sortType
            });
            return Ok(OkResponse.Create(result));
        }
    }
}
