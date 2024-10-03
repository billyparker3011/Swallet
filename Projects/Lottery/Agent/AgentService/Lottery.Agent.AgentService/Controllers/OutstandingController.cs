using HnMicro.Framework.Controllers;
using HnMicro.Framework.Enums;
using HnMicro.Framework.Responses;
using Lottery.Core.Enums;
using Lottery.Core.Filters.Authorization;
using Lottery.Core.Models.Agent.GetAgentOutstanding;
using Lottery.Core.Models.BroadCaster.GetBroadCasterOutstanding;
using Lottery.Core.Services.Agent;
using Lottery.Core.Services.BroadCaster;
using Microsoft.AspNetCore.Mvc;

namespace Lottery.Agent.AgentService.Controllers
{
    public class OutstandingController : HnControllerBase
    {
        private readonly IAgentOutstandingService _agentOutstandingService;
        private readonly IBroadCasterService _broadCasterService;

        public OutstandingController(IAgentOutstandingService agentOutstandingService, IBroadCasterService broadCasterService)
        {
            _agentOutstandingService = agentOutstandingService;
            _broadCasterService = broadCasterService;
        }

        [HttpGet, LotteryAuthorize(Permission.Report.Reports)]
        public async Task<IActionResult> GetAgentOutstandings([FromQuery] long? agentId, [FromQuery] int? roleId, [FromQuery] string sortName, [FromQuery] SortType sortType = SortType.Descending)
        {
            return Ok(OkResponse.Create(await _agentOutstandingService.GetAgentOutstandings(new GetAgentOutstandingModel
            {
                AgentId = agentId,
                RoleId = roleId,
                SortName = sortName,
                SortType = sortType
            })));
        }

        [HttpGet("broad-caster"), LotteryAuthorize(Permission.Report.Reports)]
        public async Task<IActionResult> GetBroadCasterOutstandings([FromQuery] string sortName, [FromQuery] SortType sortType = SortType.Descending)
        {
            return Ok(OkResponse.Create(await _broadCasterService.GetBroadCasterOutstandings(new GetBroadCasterOutstandingModel
            {
                SortName = sortName,
                SortType = sortType
            })));
        }
    }
}
