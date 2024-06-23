using HnMicro.Framework.Controllers;
using HnMicro.Framework.Responses;
using Lottery.Core.Enums;
using Lottery.Core.Filters.Authorization;
using Lottery.Core.Models.Match.ChangeState;
using Lottery.Core.Models.Match.CreateMatch;
using Lottery.Core.Services.Match;
using Lottery.Match.MatchService.Requests.Match;
using Microsoft.AspNetCore.Mvc;

namespace Lottery.Match.MatchService.Controllers
{
    public class MatchController : HnControllerBase
    {
        private readonly IMatchService _matchService;

        public MatchController(IMatchService matchService)
        {
            _matchService = matchService;
        }

        [HttpGet, LotteryAuthorize(Permission.Management.Matches, Permission.Management.AdvancedTickets)]
        public async Task<IActionResult> GetMatches([FromQuery] bool? displayResult)
        {
            return Ok(OkResponse.Create(await _matchService.GetMatches(30, displayResult ?? false)));
        }

        [HttpPost, LotteryAuthorize(Permission.Management.Matches)]
        public async Task<IActionResult> CreateMatch([FromBody] CreateOrUpdateMatchRequest request)
        {
            await _matchService.CreateMatch(new CreateOrUpdateMatchModel
            {
                KickOff = request.KickOff,
                IncludeTime = request.IncludeTime,
                AllowBeforeDate = request.AllowBeforeDate
            });
            return Ok();
        }

        [HttpPut("{matchId:long}/change-state"), LotteryAuthorize(Permission.Management.Matches)]
        public async Task<IActionResult> ChangeState([FromRoute] long matchId, [FromBody] ChangeStateRequest request)
        {
            await _matchService.ChangeState(matchId, new ChangeStateModel { State = request.State });
            return Ok();
        }

        [HttpGet("{matchId:long}"), LotteryAuthorize(Permission.Management.Matches)]
        public async Task<IActionResult> GetMatchById([FromRoute] long matchId)
        {
            return Ok(OkResponse.Create(await _matchService.GetMatchById(matchId)));
        }

        [HttpGet("running-match")]
        public async Task<IActionResult> GetRunningMatch()
        {
            return Ok(OkResponse.Create(await _matchService.GetRunningMatch()));
        }

        [HttpGet("results/{kickOffTime:datetime}")]
        public async Task<IActionResult> GetResults([FromRoute] DateTime kickOffTime)
        {
            return Ok(OkResponse.Create(await _matchService.ResultsByKickoff(kickOffTime)));
        }

        [HttpGet("update-running-match"), LotteryAuthorize(Permission.Management.Matches)]
        public async Task<IActionResult> UpdateRunningMatch()
        {
            await _matchService.UpdateRunningMatch();
            return Ok();
        }
    }
}