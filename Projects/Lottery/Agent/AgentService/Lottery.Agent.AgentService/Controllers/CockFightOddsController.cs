using HnMicro.Framework.Controllers;
using HnMicro.Framework.Responses;
using Lottery.Agent.AgentService.Requests.CockFight;
using Lottery.Core.Models.CockFight.UpdateCockFightAgentBetSetting;
using Lottery.Core.Services.CockFight;
using Microsoft.AspNetCore.Mvc;

namespace Lottery.Agent.AgentService.Controllers
{
    public class CockFightOddsController : HnControllerBase
    {
        private readonly ICockFightAgentBetSettingService _cockFightAgentBetSettingService;

        public CockFightOddsController(ICockFightAgentBetSettingService cockFightAgentBetSettingService)
        {
            _cockFightAgentBetSettingService = cockFightAgentBetSettingService;
        }

        [HttpGet("{agentId:long}/bet-settings")]
        public async Task<IActionResult> GetDetailCockFightAgentBetSettings([FromRoute] long agentId)
        {
            var result = await _cockFightAgentBetSettingService.GetCockFightAgentBetSettingDetail(agentId);
            return Ok(OkResponse.Create(result));
        }

        [HttpPut("{agentId:long}/bet-settings")]
        public async Task<IActionResult> UpdateCockFightAgentBetSetting([FromRoute] long agentId, [FromBody] UpdateCockFightAgentBetSettingRequest request)
        {
            await _cockFightAgentBetSettingService.UpdateCockFightAgentBetSetting(agentId, new UpdateCockFightAgentBetSettingModel
            {
                BetKindId = request.BetKindId,
                MainLimitAmountPerFight = request.MainLimitAmountPerFight,
                DrawLimitAmountPerFight = request.DrawLimitAmountPerFight,
                LimitNumTicketPerFight = request.LimitNumTicketPerFight
            });
            return Ok();
        }

        [HttpGet("default-bet-settings")]
        public async Task<IActionResult> GetDefaultCockFightCompanyBetSettings()
        {
            var result = await _cockFightAgentBetSettingService.GetDefaultCockFightCompanyBetSetting();
            return Ok(OkResponse.Create(result));
        }

        [HttpPut("default-bet-settings")]
        public async Task<IActionResult> UpdateDefaultCockFightCompanyBetSetting([FromBody] UpdateDefaultCockFightCompanyBetSettingRequest request)
        {
            await _cockFightAgentBetSettingService.UpdateDefaultCockFightCompanyBetSetting(new UpdateCockFightAgentBetSettingModel
            {
                BetKindId = request.BetKindId,
                MainLimitAmountPerFight = request.MainLimitAmountPerFight,
                DrawLimitAmountPerFight = request.DrawLimitAmountPerFight,
                LimitNumTicketPerFight = request.LimitNumTicketPerFight
            });
            return Ok();
        }
    }
}
