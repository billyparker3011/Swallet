using HnMicro.Modules.EntityFrameworkCore.Repositories;
using Lottery.Data;

namespace Lottery.Core.Repositories.Audit
{
    public class AuditRepository : EntityFrameworkCoreRepository<long, Data.Entities.Audit, LotteryContext>, IAuditRepository
    {
        public AuditRepository(LotteryContext context) : base(context)
        {
        }
    }
}
