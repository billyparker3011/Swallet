using HnMicro.Framework.Controllers;
using HnMicro.Framework.Models;
using HnMicro.Framework.Responses;
using Lottery.Agent.AgentService.Requests.Agent;
using Lottery.Core.Models.Agent.CreateAgent;
using Lottery.Core.Models.Agent.CreateSubAgent;
using Lottery.Core.Models.Agent.GetAgentCreditBalance;
using Lottery.Core.Models.Agent.GetAgents;
using Lottery.Core.Models.Agent.UpdateAgent;
using Lottery.Core.Models.Agent.UpdateAgentCreditBalance;
using Lottery.Core.Services.Agent;
using Microsoft.AspNetCore.Mvc;

namespace Lottery.Agent.AgentService.Controllers
{
    public class AgentController : HnControllerBase
    {
        private readonly IAgentService _agentService;
        private readonly IAgentBetSettingService _agentBetSettingService;
        private readonly IAgentPositionTakingService _agentPositionTakingService;

        public AgentController(IAgentService agentService, IAgentBetSettingService agentBetSettingService, IAgentPositionTakingService agentPositionTakingService)
        {
            _agentService = agentService;
            _agentBetSettingService = agentBetSettingService;
            _agentPositionTakingService = agentPositionTakingService;
        }

        [HttpPost("agent")]
        public async Task<IActionResult> CreateAgent([FromBody] CreateAgentRequest request)
        {
            await _agentService.CreateAgent(new CreateAgentModel
            {
                Username = request.Username,
                Password = request.Password,
                Credit = request.Credit,
                FirstName = request.FirstName,
                LastName = request.LastName,
                MemberMaxCredit = request.MemberMaxCredit,
                BetSettings = request.BetSettings,
                PositionTakings = request.PositionTakings
            });
            return Ok();
        }

        [HttpGet("agent/suggestion-identifier")]
        public async Task<IActionResult> GetSuggestionAgentIdentifier()
        {
            return Ok(OkResponse.Create(await _agentService.GetSuggestionAgentIdentifier(false)));
        }

        [HttpGet("agents")]
        public async Task<IActionResult> GetAgents([FromQuery] long? agentId, [FromQuery] int? state, [FromQuery] string searchTerm, [FromQuery] QueryAdvance advanceRequest)
        {
            var result = await _agentService.GetAgents(new GetAgentsModel
            {
                AgentId = agentId,
                SearchTerm = searchTerm,
                State = state,
                PageSize = advanceRequest.PageSize,
                PageIndex = advanceRequest.PageIndex,
                SortName = advanceRequest.SortName,
                SortType = advanceRequest.SortType
            });
            return Ok(OkResponse.Create(result.Agents, result.Metadata));
        }

        [HttpPut("agent/{id}")]
        public async Task<IActionResult> UpdateAgent([FromRoute] long id, [FromBody] UpdateAgentRequest request)
        {
            await _agentService.UpdateAgent(new UpdateAgentModel
            {
                AgentId = id,
                State = request.State,
                FirstName = request.FirstName,
                LastName = request.LastName,
                Permissions = request.Permissions,
                Credit = request.Credit,
                MemberMaxCredit = request.MemberMaxCredit
            });
            return Ok();
        }

        [HttpPut("agent/{agentId:long}/bet-setting")]
        public async Task<IActionResult> UpdateAgentBetSetting([FromRoute] long agentId, [FromBody] UpdateAgentBetSettingRequest request)
        {
            await _agentService.UpdateAgentBetSetting(agentId, request.BetSettings);
            return Ok();
        }

        [HttpPut("agent/{agentId:long}/position-taking")]
        public async Task<IActionResult> UpdateAgentPositionTaking([FromRoute] long agentId, [FromBody] UpdateAgentPositionTakingRequest request)
        {
            await _agentService.UpdateAgentPositionTaking(agentId, request.PositionTakings);
            return Ok();
        }

        [HttpGet("agent/exists")]
        public async Task<IActionResult> CheckExistAgent([FromQuery] string username)
        {
            return Ok(OkResponse.Create(await _agentService.CheckExistAgent(username)));
        }

        [HttpGet("agent/credit-info")]
        public async Task<IActionResult> GetAgentCreditInfo([FromQuery] long? agentId)
        {
            return Ok(OkResponse.Create(await _agentService.GetAgentCreditInfo(agentId)));
        }

        [HttpPost("sub-agent")]
        public async Task<IActionResult> CreateSubAgent([FromBody] CreateSubAgentRequest request)
        {
            await _agentService.CreateSubAgent(new CreateSubAgentModel
            {
                Username = request.Username,
                Password = request.Password,
                FirstName = request.FirstName,
                LastName = request.LastName,
                Permissions = request.Permissions
            });
            return Ok();
        }

        [HttpGet("sub-agent/suggestion-identifier")]
        public async Task<IActionResult> GetSuggestionSubAgentIdentifier()
        {
            return Ok(await _agentService.GetSuggestionAgentIdentifier(true));
        }

        [HttpGet("agent/dashboard")]
        public async Task<IActionResult> GetAgentDashBoard()
        {
            return Ok(OkResponse.Create(await _agentService.GetAgentDashBoard()));
        }

        [HttpGet("agent/new-players-of-the-month")]
        public async Task<IActionResult> GetNewPlayersOfTheMonth()
        {
            return Ok(OkResponse.Create(await _agentService.GetNewPlayersOfTheMonth()));
        }

        [HttpGet("agent/top-players-of-the-month")]
        public async Task<IActionResult> GetTopPlayersOfTheMonth()
        {
            return Ok(OkResponse.Create(await _agentService.GetTopPlayersOfTheMonth()));
        }

        [HttpGet("agent/highest-turn-over-players-of-the-month")]
        public async Task<IActionResult> GetHighestTurnOverPlayersOfTheMonth()
        {
            return Ok(OkResponse.Create(await _agentService.GetHighestTurnOverPlayersOfTheMonth()));
        }

        [HttpGet("agent/bread-crumbs")]
        public async Task<IActionResult> GetAgentBreadCrumbs([FromQuery] long? agentId, [FromQuery] int? roleId)
        {
            return Ok(OkResponse.Create(await _agentService.GetBreadCrumbs(agentId, roleId)));
        }

        [HttpGet("bet-settings")]
        public async Task<IActionResult> GetAgentBetSettings()
        {
            var result = await _agentBetSettingService.GetAgentBetSettings();
            return Ok(OkResponse.Create(result.AgentBetSettings));
        }

        [HttpGet("{agentId:long}/bet-settings")]
        public async Task<IActionResult> GetDetailAgentBetSettings([FromRoute] long agentId)
        {
            var result = await _agentBetSettingService.GetDetailAgentBetSettings(agentId);
            return Ok(OkResponse.Create(result.AgentBetSettings));
        }

        [HttpGet("{agentId:long}/position-takings")]
        public async Task<IActionResult> GetDetailAgentPositionTakings([FromRoute] long agentId)
        {
            var result = await _agentPositionTakingService.GetDetailAgentPositionTakings(agentId);
            return Ok(OkResponse.Create(result.AgentPositionTakings));
        }

        [HttpPut("bet-settings")]
        public async Task<IActionResult> ModifyAgentBetSettings([FromBody] UpdateAgentBetSettingRequest request)
        {
            await _agentBetSettingService.UpdateAgentBetSettings(request.BetSettings);
            return Ok();
        }

        [HttpGet("sub-agents")]
        public async Task<IActionResult> GetSubAgents()
        {
            var result = await _agentService.GetSubAgents();
            return Ok(OkResponse.Create(result.SubAgents));
        }

        [HttpGet("agent/winloss-summary")]
        public async Task<IActionResult> GetAgentWinLossSummary([FromQuery] long? agentId, [FromQuery] GetAgentWinLossSearchRequest searchRequest)
        {
            var result = await _agentService.GetAgentWinLossSummary(agentId, searchRequest.From, searchRequest.To);
            return Ok(OkResponse.Create(result));
        }

        [HttpGet("agent/credit-balance")]
        public async Task<IActionResult> GetAgentCreditBalance([FromQuery] long? agentId, [FromQuery] int? state, [FromQuery] string searchTerm)
        {
            var result = await _agentService.GetAgentCreditBalances(new GetAgentCreditBalanceModel
            {
                AgentId = agentId,
                SearchTerm = searchTerm,
                State = state
            });
            return Ok(OkResponse.Create(result));
        }

        [HttpGet("agent/{agentId:long}/credit-balance")]
        public async Task<IActionResult> GetCreditBalanceDetailPopup([FromRoute] long agentId)
        {
            var result = await _agentService.GetCreditBalanceDetailPopup(agentId);
            return Ok(OkResponse.Create(result));
        }

        [HttpPut("agent/{agentId:long}/credit-balance")]
        public async Task<IActionResult> ModifyAgentCreditBalance([FromRoute] long agentId, [FromBody] UpdateAgentCreditBalanceRequest request)
        {
            await _agentService.UpdateAgentCreditBalance(new UpdateAgentCreditBalanceModel
            {
                AgentId = agentId,
                Credit = request.Credit
            });
            return Ok();
        }
    }
}
