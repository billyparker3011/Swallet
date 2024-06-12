using HnMicro.Modules.EntityFrameworkCore.UnitOfWorks;
using Lottery.Data;

namespace Lottery.Core.UnitOfWorks
{
    public class LotteryUow : EntityFrameworkCoreUnitOfWork<LotteryContext>, ILotteryUow
    {
        public LotteryUow(LotteryContext context) : base(context)
        {
        }
    }
}
