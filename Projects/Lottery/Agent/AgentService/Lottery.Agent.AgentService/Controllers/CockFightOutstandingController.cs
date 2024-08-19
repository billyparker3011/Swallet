using HnMicro.Framework.Controllers;
using HnMicro.Framework.Enums;
using HnMicro.Framework.Responses;
using Lottery.Core.Enums;
using Lottery.Core.Filters.Authorization;
using Lottery.Core.Models.CockFight.GetCockFightAgentOutstanding;
using Lottery.Core.Services.CockFight;
using Microsoft.AspNetCore.Mvc;

namespace Lottery.Agent.AgentService.Controllers
{
    public class CockFightOutstandingController : HnControllerBase
    {
        private readonly ICockFightAgentOutstandingService _cockFightAgentOutstandingService;

        public CockFightOutstandingController(ICockFightAgentOutstandingService cockFightAgentOutstandingService)
        {
            _cockFightAgentOutstandingService = cockFightAgentOutstandingService;
        }

        [HttpGet, LotteryAuthorize(Permission.Report.Reports)]
        public async Task<IActionResult> GetAgentOutstandings([FromQuery] long? agentId, [FromQuery] int? roleId, [FromQuery] string sortName, [FromQuery] SortType sortType = SortType.Descending)
        {
            return Ok(OkResponse.Create(await _cockFightAgentOutstandingService.GetCockFightAgentOutstanding(new GetCockFightAgentOutstandingModel
            {
                AgentId = agentId,
                RoleId = roleId,
                SortName = sortName,
                SortType = sortType
            })));
        }
    }
}
