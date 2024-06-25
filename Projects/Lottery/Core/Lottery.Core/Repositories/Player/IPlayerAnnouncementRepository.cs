using HnMicro.Modules.EntityFrameworkCore.Repositories;
using Lottery.Data;

namespace Lottery.Core.Repositories.Player
{
    public interface IPlayerAnnouncementRepository : IEntityFrameworkCoreRepository<long, Data.Entities.PlayerAnnouncement, LotteryContext>
    {

    }
}
