using HnMicro.Modules.EntityFrameworkCore.Repositories;
using Lottery.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Lottery.Core.Repositories.Casino
{
    public class CasinoTicketEventDetailRepository : EntityFrameworkCoreRepository<long, Data.Entities.Partners.Casino.CasinoTicketEventDetail, LotteryContext>, ICasinoTicketEventDetailRepository
    {
        public CasinoTicketEventDetailRepository(LotteryContext context) : base(context)
        {
        }
    }
}
