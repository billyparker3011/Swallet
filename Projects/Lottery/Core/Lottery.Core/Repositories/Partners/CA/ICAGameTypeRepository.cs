using HnMicro.Modules.EntityFrameworkCore.Repositories;
using Lottery.Data;

namespace Lottery.Core.Repositories.Partners.CA
{
    public interface ICAGameTypeRepository : IEntityFrameworkCoreRepository<long, Data.Entities.Partners.CA.CAGameType, LotteryContext>
    {

    }
}
