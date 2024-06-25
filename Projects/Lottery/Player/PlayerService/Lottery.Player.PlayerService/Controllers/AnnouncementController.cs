using HnMicro.Framework.Controllers;
using HnMicro.Framework.Responses;
using Lottery.Core.Models.Announcement.GetAnnouncementByType;
using Lottery.Core.Services.Announcement;
using Lottery.Player.PlayerService.Requests.Announcement;
using Microsoft.AspNetCore.Mvc;

namespace Lottery.Player.PlayerService.Controllers
{
    [Route(Core.Helpers.RouteHelper.V1.Announcement.BaseRoute)]
    public class AnnouncementController : HnControllerBase
    {
        private readonly IAnnouncementService _announcementService;

        public AnnouncementController(IAnnouncementService announcementService)
        {
            _announcementService = announcementService;
        }

        [HttpGet]
        public async Task<IActionResult> GetAnnouncementByType([FromQuery] GetAnnouncementsByTypeRequest request)
        {
            var result = await _announcementService.GetAnnouncementByType(new GetAnnouncementByTypeModel
            {
                Type = request.Type,
                PageIndex = request.PageIndex,
                PageSize = request.PageSize,
                IsAgent = false
            });
            return Ok(OkResponse.Create(result.Announcements, result.Metadata));
        }

        [HttpGet("/get-unread-announcements")]
        public async Task<IActionResult> GetUnreadAnnouncements()
        {
            return Ok(OkResponse.Create(await _announcementService.GetUnreadAnnouncements(false)));
        }
    }
}
