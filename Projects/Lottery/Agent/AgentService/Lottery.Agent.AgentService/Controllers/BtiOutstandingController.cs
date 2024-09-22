using HnMicro.Framework.Controllers;
using HnMicro.Framework.Enums;
using HnMicro.Framework.Responses;
using Lottery.Core.Enums;
using Lottery.Core.Filters.Authorization;
using Lottery.Core.Partners.Models.Bti;
using Lottery.Core.Services.Partners.Bti;
using Microsoft.AspNetCore.Mvc;

namespace Lottery.Agent.AgentService.Controllers
{
    public class BtiOutstandingController : HnControllerBase
    {
        private readonly IBtiAgentOutstandingService _btiAgentOutstandingService;

        public BtiOutstandingController(IBtiAgentOutstandingService btiAgentOutstandingService)
        {
            _btiAgentOutstandingService = btiAgentOutstandingService;
        }

        [HttpGet, LotteryAuthorize(Permission.Report.Reports)]
        public async Task<IActionResult> GetAgentOutstandings([FromQuery] long? agentId, [FromQuery] int? roleId, [FromQuery] string sortName, [FromQuery] SortType sortType = SortType.Descending)
        {
            return Ok(OkResponse.Create(await _btiAgentOutstandingService.GetBtiAgentOutstanding(new GetBtiAgentOutstandingModel
            {
                AgentId = agentId,
                RoleId = roleId,
                SortName = sortName,
                SortType = sortType
            })));
        }
    }
}
