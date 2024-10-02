using HnMicro.Framework.Responses;
using Lottery.Core.Dtos.Announcement;

namespace Lottery.Core.Models.Announcement.GetAnnouncementByType
{
    public class GetAnnouncementByTypeResult
    {
        public List<AnnouncementDto> Announcements { get; set; }
        public ApiResponseMetadata Metadata { get; set; }
    }
}
