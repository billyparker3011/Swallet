using HnMicro.Core.Scopes;
using Lottery.Core.Models.Announcement.CreateAnnouncement;
using Lottery.Core.Models.Announcement.GetAnnouncementByType;
using Lottery.Core.Models.Announcement.GetUnreadAnnouncements;
using Lottery.Core.Models.Announcement.UpdateAnnouncement;

namespace Lottery.Core.Services.Announcement
{
    public interface IAnnouncementService : IScopedDependency
    {
        Task CreateAnnouncement(CreateAnnouncementModel model);
        Task<GetAnnouncementByTypeResult> GetAnnouncementByType(GetAnnouncementByTypeModel query);
        Task UpdateAnnouncement(UpdateAnnouncementModel updateModel);
        Task DeleteAnnouncement(long id);
        Task<GetUnreadAnnouncementsResult> GetUnreadAnnouncements();
    }
}
