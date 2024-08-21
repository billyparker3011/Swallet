using HnMicro.Modules.EntityFrameworkCore.Repositories;
using Lottery.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Lottery.Core.Repositories.Casino
{
    public interface ICasinoTicketEventDetailRepository : IEntityFrameworkCoreRepository<long, Data.Entities.Partners.Casino.CasinoTicketEventDetail, LotteryContext>
    {

    }
}
