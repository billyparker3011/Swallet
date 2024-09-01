using HnMicro.Framework.Controllers;
using HnMicro.Framework.Enums;
using HnMicro.Framework.Responses;
using Lottery.Core.Enums;
using Lottery.Core.Filters.Authorization;
using Lottery.Core.Services.Partners.CA;
using Microsoft.AspNetCore.Mvc;

namespace Lottery.Agent.AgentService.Controllers
{
    public class CasinoOutstandingController : HnControllerBase
    {
        private readonly ICasinoAgentOutstandingService _casinoAgentOutstandingService;

        public CasinoOutstandingController(ICasinoAgentOutstandingService casinoAgentOutstandingService)
        {
            _casinoAgentOutstandingService = casinoAgentOutstandingService;
        }

        [HttpGet, LotteryAuthorize(Permission.Report.Reports)]
        public async Task<IActionResult> GetAgentOutstandings([FromQuery] long? agentId, [FromQuery] int? roleId, [FromQuery] string sortName, [FromQuery] SortType sortType = SortType.Descending)
        {
            return Ok(OkResponse.Create(await _casinoAgentOutstandingService.GetCasinoAgentOutstanding(new Core.Partners.Models.Allbet.GetCasinoAgentOutstandingModel
            {
                AgentId = agentId,
                RoleId = roleId,
                SortName = sortName,
                SortType = sortType
            })));
        }
    }
}
