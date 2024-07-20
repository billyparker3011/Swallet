using HnMicro.Framework.Controllers;
using HnMicro.Framework.Responses;
using Lottery.Agent.AgentService.Requests.Agent;
using Lottery.Core.Services.Agent;
using Lottery.Core.Services.BroadCaster;
using Microsoft.AspNetCore.Mvc;

namespace Lottery.Agent.AgentService.Controllers
{
    public class WinlossController : HnControllerBase
    {
        private readonly IBroadCasterService _broadCasterService;
        private readonly IAgentService _agentService;

        public WinlossController(IBroadCasterService broadCasterService, IAgentService agentService)
        {
            _broadCasterService = broadCasterService;
            _agentService = agentService;
        }

        [HttpGet("agent/winloss-summary")]
        public async Task<IActionResult> GetAgentWinLossSummary([FromQuery] long? agentId, [FromQuery] GetAgentWinLossSearchRequest searchRequest)
        {
            return Ok(OkResponse.Create(await _agentService.GetAgentWinLossSummary(agentId, searchRequest.From, searchRequest.To, searchRequest.SelectedDraft)));
        }

        [HttpGet("broad-caster/winloss-summary")]
        public async Task<IActionResult> GetBroadCasterWinLossSummary([FromQuery] GetAgentWinLossSearchRequest searchRequest)
        {
            return Ok(OkResponse.Create(await _broadCasterService.GetBroadCasterWinLossSummary(searchRequest.From, searchRequest.To, searchRequest.SelectedDraft)));
        }
    }
}
