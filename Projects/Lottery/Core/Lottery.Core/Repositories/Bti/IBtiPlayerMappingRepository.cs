using HnMicro.Modules.EntityFrameworkCore.Repositories;
using Lottery.Data;

namespace Lottery.Core.Repositories.Bti
{
    public interface IBtiPlayerMappingRepository : IEntityFrameworkCoreRepository<long, Data.Entities.Partners.Bti.BtiPlayerMapping, LotteryContext>
    {

    }
}
