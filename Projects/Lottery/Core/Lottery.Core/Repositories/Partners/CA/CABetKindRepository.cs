using HnMicro.Modules.EntityFrameworkCore.Repositories;
using Lottery.Core.Repositories.BetKind;
using Lottery.Data;

namespace Lottery.Core.Repositories.Partners.CA
{
    public class CABetKindRepository : EntityFrameworkCoreRepository<long, Data.Entities.Partners.CA.CABetKind, LotteryContext>, ICABetKindRepository
    {
        public CABetKindRepository(LotteryContext context) : base(context)
        {
        }

    }
}
