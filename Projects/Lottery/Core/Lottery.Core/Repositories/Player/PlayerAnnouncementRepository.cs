using HnMicro.Modules.EntityFrameworkCore.Repositories;
using Lottery.Data;

namespace Lottery.Core.Repositories.Player
{
    public class PlayerAnnouncementRepository : EntityFrameworkCoreRepository<long, Data.Entities.PlayerAnnouncement, LotteryContext>, IPlayerAnnouncementRepository
    {
        public PlayerAnnouncementRepository(LotteryContext context) : base(context)
        {
        }
    }
}
