using HnMicro.Modules.EntityFrameworkCore.Repositories;
using Lottery.Data;

namespace Lottery.Core.Repositories.Casino
{
    public interface ICasinoGameTypeRepository : IEntityFrameworkCoreRepository<int, Data.Entities.Partners.Casino.CasinoGameType, LotteryContext>
    {

    }
}
