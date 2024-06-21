using HnMicro.Modules.EntityFrameworkCore.Repositories;
using Lottery.Data;

namespace Lottery.Core.Repositories.Announcement
{
    public interface IAnnouncementRepository : IEntityFrameworkCoreRepository<long, Data.Entities.Announcement, LotteryContext>
    {
    }
}
