using HnMicro.Modules.EntityFrameworkCore.Repositories;
using Lottery.Data;

namespace Lottery.Core.Repositories.Player
{
    public class PlayerAuditRepository : EntityFrameworkCoreRepository<long, Data.Entities.PlayerAudit, LotteryContext>, IPlayerAuditRepository
    {
        public PlayerAuditRepository(LotteryContext context) : base(context)
        {
        }
    }
}
