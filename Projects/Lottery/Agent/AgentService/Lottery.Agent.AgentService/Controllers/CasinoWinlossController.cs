using HnMicro.Framework.Controllers;
using HnMicro.Framework.Responses;
using Lottery.Agent.AgentService.Requests.Casino;
using Lottery.Core.Enums;
using Lottery.Core.Filters.Authorization;
using Lottery.Core.Services.Partners.CA;
using Microsoft.AspNetCore.Mvc;

namespace Lottery.Agent.AgentService.Controllers
{

    public class CasinoWinlossController : HnControllerBase
    {
        private readonly ICasinoAgentWinlossService _CasinoAgentWinlossService;

        public CasinoWinlossController(ICasinoAgentWinlossService CasinoAgentWinlossService)
        {
            _CasinoAgentWinlossService = CasinoAgentWinlossService;
        }

        [HttpGet, LotteryAuthorize(Permission.Report.Reports)]
        public async Task<IActionResult> GetCasinoAgentWinLossSummary([FromQuery] long? agentId, [FromQuery] GetCasinoAgentWinLossSearchRequest searchRequest)
        {
            return Ok(OkResponse.Create(await _CasinoAgentWinlossService.GetCasinoAgentWinloss(agentId, searchRequest.From, searchRequest.To)));
        }
    }
}
