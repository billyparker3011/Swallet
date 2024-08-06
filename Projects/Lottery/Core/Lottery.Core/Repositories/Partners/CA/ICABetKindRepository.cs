using HnMicro.Modules.EntityFrameworkCore.Repositories;
using Lottery.Core.Repositories.BetKind;
using Lottery.Data;

namespace Lottery.Core.Repositories.Partners.CA
{
    public interface ICABetKindRepository : IEntityFrameworkCoreRepository<long, Data.Entities.Partners.CA.CABetKind, LotteryContext>
    {

    }
}
