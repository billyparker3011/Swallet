using HnMicro.Modules.EntityFrameworkCore.Repositories;
using Lottery.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Lottery.Core.Repositories.Audit
{
    public interface IAuditRepository : IEntityFrameworkCoreRepository<long, Data.Entities.Audit, LotteryContext>
    {
    }
}
