using HnMicro.Framework.Controllers;
using HnMicro.Framework.Responses;
using Lottery.Agent.AgentService.Requests.CockFight;
using Lottery.Core.Enums;
using Lottery.Core.Filters.Authorization;
using Lottery.Core.Services.CockFight;
using Microsoft.AspNetCore.Mvc;

namespace Lottery.Agent.AgentService.Controllers
{
    public class CockFightWinlossController : HnControllerBase
    {
        private readonly ICockFightAgentWinlossService _cockFightAgentWinlossService;

        public CockFightWinlossController(ICockFightAgentWinlossService cockFightAgentWinlossService)
        {
            _cockFightAgentWinlossService = cockFightAgentWinlossService;
        }

        [HttpGet, LotteryAuthorize(Permission.Report.Reports)]
        public async Task<IActionResult> GetCockFightAgentWinLossSummary([FromQuery] long? agentId, [FromQuery] GetCockFightAgentWinLossSearchRequest searchRequest)
        {
            return Ok(OkResponse.Create(await _cockFightAgentWinlossService.GetCockFightAgentWinloss(agentId, searchRequest.From, searchRequest.To)));
        }
    }
}
