using HnMicro.Core.Scopes;
using Lottery.Tools.AdjustOddsService.Services.AdjustOdds.Commands;

namespace Lottery.Tools.AdjustOddsService.Services.AdjustOdds
{
    public interface IOddsAdjustmentService : ISingletonDependency
    {
        void Enqueue(AdjustOddsCommand command);
    }
}
