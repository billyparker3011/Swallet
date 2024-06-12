using HnMicro.Framework.Controllers;
using HnMicro.Framework.Responses;
using Lottery.Agent.AgentService.Requests.BetKind;
using Lottery.Core.Enums;
using Lottery.Core.Filters.Authorization;
using Lottery.Core.Services.BetKind;
using Microsoft.AspNetCore.Mvc;

namespace Lottery.Agent.AgentService.Controllers
{
    [Route(Core.Helpers.RouteHelper.V1.BetKind.BaseRoute)]
    public class BetKindController : HnControllerBase
    {
        public readonly IBetKindService _betKindService;

        public BetKindController(IBetKindService betKindService)
        {
            _betKindService = betKindService;
        }

        [HttpGet("get-filters"), LotteryAuthorize(Permission.Management.BetKinds)]
        public IActionResult GetBetKindFilters()
        {
            return Ok(OkResponse.Create(_betKindService.GetFilterDatas()));
        }

        [HttpGet, LotteryAuthorize(Permission.Management.BetKinds, Permission.Management.AdvancedTickets)]
        public async Task<IActionResult> GetBetKinds([FromQuery] int? regionId, [FromQuery] int? categoryId)
        {
            return Ok(OkResponse.Create(await _betKindService.GetBetKinds(regionId, categoryId)));
        }

        [HttpPost, LotteryAuthorize(Permission.Management.BetKinds)]
        public async Task<IActionResult> UpdateBetKinds([FromBody] ModifyBetKindRequest request)
        {
            await _betKindService.UpdateBetKinds(request.ModifiedBetKinds);
            return Ok();
        }
    }
}
