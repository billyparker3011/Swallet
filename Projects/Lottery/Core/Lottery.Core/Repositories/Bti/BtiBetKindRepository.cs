using HnMicro.Modules.EntityFrameworkCore.Repositories;
using Lottery.Data;

namespace Lottery.Core.Repositories.Bti
{
    public class BtiBetKindRepository : EntityFrameworkCoreRepository<long, Data.Entities.Partners.Bti.BtiBetKind, LotteryContext>, IBtiBetKindRepository
    {
        public BtiBetKindRepository(LotteryContext context) : base(context)
        {
        }
    }
}
