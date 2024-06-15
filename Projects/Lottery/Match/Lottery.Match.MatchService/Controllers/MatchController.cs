using HnMicro.Framework.Controllers;
using HnMicro.Framework.Responses;
using Lottery.Core.Enums;
using Lottery.Core.Filters.Authorization;
using Lottery.Core.Models.Match;
using Lottery.Core.Models.Match.ChangeState;
using Lottery.Core.Models.Match.CreateMatch;
using Lottery.Core.Models.MatchResult;
using Lottery.Core.Services.Match;
using Lottery.Match.MatchService.Requests.Match;
using Lottery.Match.MatchService.Requests.MatchResult;
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
        public async Task<IActionResult> GetMatches([FromQuery] bool displayResult = false)
        {
            return Ok(OkResponse.Create(await _matchService.GetMatches(30, displayResult)));
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

        [HttpPut("{matchId:long}/on-off-process-ticket"), LotteryAuthorize(Permission.Management.Matches)]
        public async Task<IActionResult> OnOffProcessTicketOfChannel([FromRoute] long matchId, [FromBody] OnOffProcessTicketOfChannelRequest request)
        {
            await _matchService.OnOffProcessTicketOfChannel(new OnOffProcessTicketOfChannelModel
            {
                MatchId = matchId,
                RegionId = request.RegionId,
                ChannelId = request.ChannelId
            });
            return Ok();
        }

        [HttpGet("running-match")]
        public async Task<IActionResult> GetRunningMatch()
        {
            return Ok(OkResponse.Create(await _matchService.GetRunningMatch()));
        }

        [HttpPut("{matchId:long}/match-result/{regionId:int}/channels/{channelId:int}")]
        public async Task<IActionResult> UpdateMatchResult([FromRoute] long matchId, [FromRoute] int regionId, [FromRoute] int channelId, [FromBody] MatchResultRequest request)
        {
            await _matchService.UpdateMatchResults(new MatchResultModel
            {
                MatchId = matchId,
                RegionId = regionId,
                ChannelId = channelId,
                IsLive = request.IsLive,
                Results = request.Results.Select(f => new PrizeMatchResultModel
                {
                    Prize = f.Prize,
                    Order = f.Order,
                    EnabledProcessTicket = f.EnabledProcessTicket,
                    Results = f.Results.Select(f1 => new PrizeMatchResultDetailModel
                    {
                        Position = f1.Position,
                        Result = f1.Result
                    }).ToList()
                }).ToList()
            });
            return Ok();
        }

        [HttpGet("results/{kickOffTime:datetime}")]
        public async Task<IActionResult> GetResults([FromRoute] DateTime kickOffTime)
        {
            return Ok(OkResponse.Create(await _matchService.ResultsByKickoff(kickOffTime)));
        }
    }
}