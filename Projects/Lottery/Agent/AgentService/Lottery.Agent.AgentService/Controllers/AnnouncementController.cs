using HnMicro.Framework.Controllers;
using HnMicro.Framework.Responses;
using Lottery.Agent.AgentService.Requests.Announcement;
using Lottery.Core.Models.Announcement.CreateAnnouncement;
using Lottery.Core.Models.Announcement.GetAnnouncementByType;
using Lottery.Core.Models.Announcement.UpdateAnnouncement;
using Lottery.Core.Services.Announcement;
using Microsoft.AspNetCore.Mvc;

namespace Lottery.Agent.AgentService.Controllers
{
    [Route(Core.Helpers.RouteHelper.V1.Announcement.BaseRoute)]
    public class AnnouncementController : HnControllerBase
    {
        private readonly IAnnouncementService _announcementService;

        public AnnouncementController(IAnnouncementService announcementService)
        {
            _announcementService = announcementService;
        }

        [HttpPost]
        public async Task<IActionResult> CreateAnnouncement([FromQuery] int type, [FromBody] CreateAnnouncementRequest request)
        {
            await _announcementService.CreateAnnouncement(new CreateAnnouncementModel
            {
                AnnouncementType = type,
                AnnouncementLevel = request.Level,
                AnnouncementContent = request.Content,
                AnnouncementReceivers = request.AnnouncementReceivers
            });
            return Ok();
        }

        [HttpGet]
        public async Task<IActionResult> GetAnnouncementByType([FromQuery] GetAnnouncementsByTypeRequest request)
        {
            var result = await _announcementService.GetAnnouncementByType(new GetAnnouncementByTypeModel
            {
                Type = request.Type,
                PageIndex = request.PageIndex,
                PageSize = request.PageSize,
                IsAgent = true
            });
            return Ok(OkResponse.Create(result.Announcements, result.Metadata));
        }

        [HttpPut("/{id:long}")]
        public async Task<IActionResult> UpdateAnnouncement([FromRoute] long id, [FromBody] UpdateAnnouncementRequest request)
        {
            await _announcementService.UpdateAnnouncement(new UpdateAnnouncementModel
            {
                AnnouncementId = id,
                Level = request.Level,
                Content = request.Content
            });
            return Ok();
        }

        [HttpDelete("/{id:long}")]
        public async Task<IActionResult> DeleteAnnouncement([FromRoute] long id)
        {
            await _announcementService.DeleteAnnouncement(id);
            return Ok();
        }

        [HttpGet("/get-unread-announcements")]
        public async Task<IActionResult> GetUnreadAnnouncements()
        {
            return Ok(OkResponse.Create(await _announcementService.GetUnreadAnnouncements(true)));
        }

        [HttpPost("/delete-multi-announcements")]
        public async Task<IActionResult> DeleteMultipleAnnouncements([FromBody] List<long> selectedIds)
        {
            await _announcementService.DeleteMultipleAnnouncement(selectedIds);
            return Ok();
        }
    }
}
