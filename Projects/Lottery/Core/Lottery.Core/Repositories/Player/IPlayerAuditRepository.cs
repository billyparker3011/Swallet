using HnMicro.Modules.EntityFrameworkCore.Repositories;
using Lottery.Data;

namespace Lottery.Core.Repositories.Player
{
    public interface IPlayerAuditRepository : IEntityFrameworkCoreRepository<long, Data.Entities.PlayerAudit, LotteryContext>
    {

    }
}
