using HnMicro.Modules.EntityFrameworkCore.Repositories;
using Lottery.Data;

namespace Lottery.Core.Repositories.Announcement
{
    public class AgentAnnouncementRepository : EntityFrameworkCoreRepository<long, Data.Entities.AgentAnnouncement, LotteryContext>, IAgentAnnouncementRepository
    {
        public AgentAnnouncementRepository(LotteryContext context) : base (context) 
        { }
    }
}
