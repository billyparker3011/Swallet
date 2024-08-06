using HnMicro.Modules.EntityFrameworkCore.Repositories;
using Lottery.Data;

namespace Lottery.Core.Repositories.Partners.CA
{
    public class CAGameTypeRepository : EntityFrameworkCoreRepository<long, Data.Entities.Partners.CA.CAGameType, LotteryContext>, ICAGameTypeRepository
    {
        public CAGameTypeRepository(LotteryContext context) : base(context)
        {
        }

    }
}
