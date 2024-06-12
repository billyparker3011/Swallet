using HnMicro.Framework.Controllers;
using HnMicro.Framework.Responses;
using Lottery.Agent.AgentService.Requests.Odds;
using Lottery.Core.Enums;
using Lottery.Core.Filters.Authorization;
using Lottery.Core.Models.Odds;
using Lottery.Core.Services.Odds;
using Microsoft.AspNetCore.Mvc;

namespace Lottery.Agent.AgentService.Controllers
{
    public class OddsController : HnControllerBase
    {
        private readonly IOddsService _oddsService;

        public OddsController(IOddsService oddsService)
        {
            _oddsService = oddsService;
        }

        [HttpGet("default-odds")]
        public async Task<IActionResult> GetDefaultOdds()
        {
            return Ok(OkResponse.Create(await _oddsService.GetDefaultOdds()));
        }

        [HttpPost("update-default-odds"), LotteryAuthorize(Permission.Management.DefaultOdds)]
        public async Task<IActionResult> UpdateDefaultOdds([FromBody] UpdateOddsRequest request)
        {
            await _oddsService.UpdateAgentOdds(request.Odds.Select(f => new OddsModel
            {
                Id = f.Id,
                BetKindId = f.BetKindId,
                Buy = f.Buy,
                MinBuy = f.MinBuy,
                MaxBuy = f.MaxBuy,
                MinBet = f.MinBet,
                MaxBet = f.MaxBet,
                MaxPerNumber = f.MaxPerNumber
            }).ToList(), true);
            return Ok();
        }

        [HttpGet("odds-table")]
        public async Task<IActionResult> OddsTable([FromQuery] int betKindId)
        {
            return Ok(OkResponse.Create(await _oddsService.GetOddsTableByBetKind(betKindId)));
        }

        [HttpPut("odds-table/{matchId:long}/{betKindId:int}")]
        public async Task<IActionResult> ChangeOddsValueOfOddsTable([FromRoute] long matchId, [FromRoute] int betKindId, [FromBody] ChangeOddsValueOfOddsTableRequest request)
        {
            await _oddsService.ChangeOddsValueOfOddsTable(new ChangeOddsValueOfOddsTableModel
            {
                MatchId = matchId,
                BetKindId = betKindId,
                Number = request.Number,
                Increment = request.Increment,
                Rate = request.Rate
            });
            return Ok();
        }
    }
}
