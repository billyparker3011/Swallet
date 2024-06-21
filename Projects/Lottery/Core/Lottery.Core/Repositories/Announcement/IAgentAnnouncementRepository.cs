using HnMicro.Modules.EntityFrameworkCore.Repositories;
using Lottery.Data;

namespace Lottery.Core.Repositories.Announcement
{
    public interface IAgentAnnouncementRepository : IEntityFrameworkCoreRepository<long, Data.Entities.AgentAnnouncement, LotteryContext> { }
}
