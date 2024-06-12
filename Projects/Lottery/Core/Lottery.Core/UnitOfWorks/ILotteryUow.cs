using HnMicro.Modules.EntityFrameworkCore.UnitOfWorks;
using Lottery.Data;

namespace Lottery.Core.UnitOfWorks
{
    public interface ILotteryUow : IEntityFrameworkCoreUnitOfWork<LotteryContext>
    {
    }
}
