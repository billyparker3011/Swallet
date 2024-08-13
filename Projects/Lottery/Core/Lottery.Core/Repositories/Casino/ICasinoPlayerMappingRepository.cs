using HnMicro.Modules.EntityFrameworkCore.Repositories;
using Lottery.Data;

namespace Lottery.Core.Repositories.Casino
{
    public interface ICasinoPlayerMappingRepository : IEntityFrameworkCoreRepository<long, Data.Entities.Partners.Casino.CasinoPlayerMapping, LotteryContext>
    {

    }
}
