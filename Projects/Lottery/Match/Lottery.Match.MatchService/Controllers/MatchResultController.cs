using HnMicro.Framework.Controllers;
using Lottery.Core.Enums;
using Lottery.Core.Filters.Authorization;
using Lottery.Core.Models.Match;
using Lottery.Core.Models.MatchResult;
using Lottery.Core.Services.Match;
using Lottery.Match.MatchService.Requests.MatchResult;
using Microsoft.AspNetCore.Mvc;

namespace Lottery.Match.MatchService.Controllers
{
    [Route(Core.Helpers.RouteHelper.V1.MatchResult.BaseRoute)]
    public class MatchResultController : HnControllerBase
    {
        private readonly IMatchService _matchService;

        public MatchResultController(IMatchService matchService)
        {
            _matchService = matchService;
        }

        [HttpGet("{matchId:long}/start-stop-process-ticket/{regionId:int}/channels/{channelId:int}"), LotteryAuthorize(Permission.Management.Matches)]
        public async Task<IActionResult> StartStopProcessTicket([FromRoute] long matchId, [FromRoute] int regionId, [FromRoute] int channelId)
        {
            await _matchService.StartStopProcessTicket(new StartStopProcessTicketModel
            {
                MatchId = matchId,
                RegionId = regionId,
                ChannelId = channelId
            });
            return Ok();
        }

        [HttpGet("{matchId:long}/start-live/{regionId:int}/channels/{channelId:int}"), LotteryAuthorize(Permission.Management.Matches)]
        public async Task<IActionResult> StartStopLive([FromRoute] long matchId, [FromRoute] int regionId, [FromRoute] int channelId)
        {
            await _matchService.StartStopLive(new StartStopLiveModel
            {
                MatchId = matchId,
                RegionId = regionId,
                ChannelId = channelId
            });
            return Ok();
        }

        [HttpPut("{matchId:long}/results/{regionId:int}/channels/{channelId:int}"), LotteryAuthorize(Permission.Management.Matches)]
        public async Task<IActionResult> UpdateResult([FromRoute] long matchId, [FromRoute] int regionId, [FromRoute] int channelId, [FromBody] UpdateResultRequest request)
        {
            await _matchService.UpdateResult(new UpdateResultModel
            {
                MatchId = matchId,
                RegionId = regionId,
                ChannelId = channelId,
                Results = request.Results.Select(f => new PrizeResultModel
                {
                    Prize = f.Prize,
                    Order = f.Order,
                    Results = f.Results.Select(f1 => new PrizeResultDetailModel
                    {
                        AllowProcessTicket = f1.AllowProcessTicket,
                        Position = f1.Position,
                        Result = f1.Result
                    }).ToList()
                }).ToList()
            });
            return Ok();
        }
    }
}