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
        private readonly INumberService _numberService;
        private readonly IProcessOddsService _processOddsService;

        public OddsController(IOddsService oddsService, INumberService numberService, IProcessOddsService processOddsService)
        {
            _oddsService = oddsService;
            _numberService = numberService;
            _processOddsService = processOddsService;
        }

        [HttpGet("default-odds")]
        public async Task<IActionResult> GetDefaultOdds()
        {
            return Ok(OkResponse.Create(await _oddsService.GetDefaultOdds()));
        }

        [HttpPost("update-default-odds"), LotteryAuthorize(Permission.Management.DefaultBetSetting)]
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

        [HttpGet("mixed-odds-table")]
        public async Task<IActionResult> MixedOddsTable([FromQuery] int betKindId)
        {
            return Ok(OkResponse.Create(await _oddsService.GetMixedOddsTableByBetKind(betKindId)));
        }

        [HttpGet("mixed-odds-table/{matchId:long}/{betKindId:int}")]
        public async Task<IActionResult> MixedOddsTableDetail([FromRoute] long matchId, [FromRoute] int betKindId, [FromQuery] int? top)
        {
            return Ok(OkResponse.Create(await _processOddsService.GetMixedOddsTableDetail(matchId, betKindId, top ?? 30)));
        }

        [HttpGet("mixed-odds-table/{matchId:long}/related-betkind/{betKindId:int}")]
        public async Task<IActionResult> MixedOddsTableRelatedBetKind([FromRoute] long matchId, [FromRoute] int betKindId, [FromQuery] int? top)
        {
            return Ok(OkResponse.Create(await _processOddsService.GetMixedOddsTableRelatedBetKind(matchId, betKindId, top ?? 10)));
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

        [HttpGet("odds-table/{matchId:long}/{betKindId:int}/suspended-numbers")]
        public async Task<IActionResult> GetSuspendedNumbers([FromRoute] long matchId, [FromRoute] int betKindId)
        {
            return Ok(OkResponse.Create(await _numberService.GetSuspendedNumbersByMatchAndBetKind(matchId, betKindId)));
        }

        [HttpPost("odds-table/{matchId:long}/{betKindId:int}/suspended-numbers")]
        public async Task<IActionResult> SuspendedNumbers([FromRoute] long matchId, [FromRoute] int betKindId, [FromBody] SuspendedNumbersRequest request)
        {
            await _numberService.AddSuspendedNumbers(new AddSuspendedNumbersModel
            {
                MatchId = matchId,
                BetKindId = betKindId,
                Numbers = request.Numbers
            });
            return Ok();
        }

        [HttpDelete("odds-table/{matchId:long}/{betKindId:int}/suspended-numbers/{number:int}")]
        public async Task<IActionResult> DeleteSuspendedNumber([FromRoute] long matchId, [FromRoute] int betKindId, [FromRoute] int number)
        {
            await _numberService.DeleteSuspendedNumber(new DeleteSuspendedNumberModel
            {
                MatchId = matchId,
                BetKindId = betKindId,
                Number = number
            });
            return Ok();
        }
    }
}
