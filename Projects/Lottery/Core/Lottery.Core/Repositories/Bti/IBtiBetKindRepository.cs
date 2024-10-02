using HnMicro.Modules.EntityFrameworkCore.Repositories;
using Lottery.Data;

namespace Lottery.Core.Repositories.Bti
{
    public interface IBtiBetKindRepository : IEntityFrameworkCoreRepository<long, Data.Entities.Partners.Bti.BtiBetKind, LotteryContext>
    {

    }
}
