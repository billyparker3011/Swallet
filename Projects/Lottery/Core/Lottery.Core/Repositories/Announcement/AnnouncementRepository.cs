using HnMicro.Modules.EntityFrameworkCore.Repositories;
using Lottery.Data;

namespace Lottery.Core.Repositories.Announcement
{
    public class AnnouncementRepository : EntityFrameworkCoreRepository<long, Data.Entities.Announcement, LotteryContext>, IAnnouncementRepository
    {
        public AnnouncementRepository(LotteryContext context) : base(context)
        {
        }
    }
}
