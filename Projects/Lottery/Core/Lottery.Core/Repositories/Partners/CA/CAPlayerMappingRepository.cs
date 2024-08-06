using HnMicro.Modules.EntityFrameworkCore.Repositories;
using Lottery.Data;

namespace Lottery.Core.Repositories.Partners.CA
{
    public class CAPlayerMappingRepository : EntityFrameworkCoreRepository<long, Data.Entities.Partners.CA.CAPlayerMapping, LotteryContext>, ICAPlayerMappingRepository
    {
        public CAPlayerMappingRepository(LotteryContext context) : base(context)
        {

        }

    }
}
