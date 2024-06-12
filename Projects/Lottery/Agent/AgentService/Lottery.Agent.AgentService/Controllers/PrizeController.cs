using HnMicro.Framework.Controllers;
using HnMicro.Framework.Responses;
using Lottery.Agent.AgentService.Requests.Prize;
using Lottery.Core.Enums;
using Lottery.Core.Filters.Authorization;
using Lottery.Core.Models.Prize;
using Lottery.Core.Services.Prizes;
using Microsoft.AspNetCore.Mvc;

namespace Lottery.Agent.AgentService.Controllers
{
    [Route(Core.Helpers.RouteHelper.V1.Prize.BaseRoute)]
    public class PrizeController : HnControllerBase
    {
        private readonly IPrizeService _prizeService;

        public PrizeController(IPrizeService prizeService)
        {
            _prizeService = prizeService;
        }

        [HttpGet("filter-options")]
        public IActionResult GetFilterOptions()
        {
            return Ok(OkResponse.Create(_prizeService.GetFilterOptions()));
        }

        [HttpGet, LotteryAuthorize(Permission.Management.Prizes)]
        public async Task<IActionResult> GetPrizes([FromQuery] int? regionId)
        {
            return Ok(OkResponse.Create(await _prizeService.GetPrizes(regionId)));
        }

        [HttpPost, LotteryAuthorize(Permission.Management.Prizes)]
        public async Task<IActionResult> UpdatePrizes([FromBody] UpdatePrizesRequest request)
        {
            await _prizeService.UpdatePrizes(new UpdatePrizesModel
            {
                Items = request.Items.Select(f => new UpdatePrizeItemModel
                {
                    Id = f.Id,
                    Name = f.Name,
                    Order = f.Order
                }).ToList()
            });
            return Ok();
        }
    }
}
