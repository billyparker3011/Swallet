using HnMicro.Framework.Controllers;
using HnMicro.Framework.Responses;
using Lottery.Agent.AgentService.Requests.Bti;
using Lottery.Core.Enums;
using Lottery.Core.Filters.Authorization;
using Lottery.Core.Services.Partners.Bti;
using Microsoft.AspNetCore.Mvc;

namespace Lottery.Agent.AgentService.Controllers
{
    public class BtiWinlossController : HnControllerBase
    {
        private readonly IBtiAgentWinlossService _btiAgentWinlossService;

        public BtiWinlossController(IBtiAgentWinlossService btiAgentWinlossService)
        {
            _btiAgentWinlossService = btiAgentWinlossService;
        }

        [HttpGet, LotteryAuthorize(Permission.Report.Reports)]
        public async Task<IActionResult> GetBtiAgentWinLossSummary([FromQuery] long? agentId, [FromQuery] GetBtiAgentWinLossSearchRequest searchRequest)
        {
            return Ok(OkResponse.Create(await _btiAgentWinlossService.GetBtiAgentWinloss(agentId, searchRequest.From, searchRequest.To)));
        }
    }
}
