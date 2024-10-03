using HnMicro.Framework.Controllers;
using HnMicro.Framework.Responses;
using Lottery.Agent.AgentService.Requests.Channel;
using Lottery.Core.Enums;
using Lottery.Core.Filters.Authorization;
using Lottery.Core.Models.Channel;
using Lottery.Core.Services.Channel;
using Microsoft.AspNetCore.Mvc;

namespace Lottery.Agent.AgentService.Controllers
{
    [Route(Core.Helpers.RouteHelper.V1.Channel.BaseRoute)]
    public class ChannelController : HnControllerBase
    {
        private readonly IChannelService _channelService;

        public ChannelController(IChannelService channelService)
        {
            _channelService = channelService;
        }

        [HttpGet("filter-options")]
        public IActionResult GetFilterOptions()
        {
            return Ok(OkResponse.Create(_channelService.GetFilterOptions()));
        }

        [HttpGet("refresh"), LotteryAuthorize(Permission.Management.Channels)]
        public async Task<IActionResult> Refresh()
        {
            await _channelService.Refresh();
            return Ok();
        }

        [HttpGet, LotteryAuthorize(Permission.Management.Channels)]
        public async Task<IActionResult> GetChannels([FromQuery] int? regionId)
        {
            return Ok(OkResponse.Create(await _channelService.GetChannels(regionId)));
        }

        [HttpPost, LotteryAuthorize(Permission.Management.Channels)]
        public async Task<IActionResult> UpdateChannels([FromBody] UpdateChannelsRequest request)
        {
            await _channelService.UpdateChannels(new UpdateChannelsModel
            {
                Items = request.Items.Select(f => new UpdateChannelItemModel
                {
                    ChannelId = f.ChannelId,
                    Name = f.Name,
                }).ToList()
            });
            return Ok();
        }
    }
}
