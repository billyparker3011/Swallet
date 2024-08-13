using HnMicro.Modules.EntityFrameworkCore.Repositories;
using Lottery.Data;

namespace Lottery.Core.Repositories.Casino
{
    public interface ICasinoBetKindRepository : IEntityFrameworkCoreRepository<int, Data.Entities.Partners.Casino.CasinoBetKind, LotteryContext>
    {

    }
}
