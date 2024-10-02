using HnMicro.Core.Scopes;
using Lottery.Core.Models.BroadCaster.GetBroadCasterOutstanding;
using Lottery.Core.Models.BroadCaster.GetBroadCasterWinlossSummary;

namespace Lottery.Core.Services.BroadCaster
{
    public interface IBroadCasterService : IScopedDependency
    {
        Task<GetBroadCasterOutstandingResult> GetBroadCasterOutstandings(GetBroadCasterOutstandingModel model);
        Task<GetBroadCasterWinlossSummaryResult> GetBroadCasterWinLossSummary(DateTime from, DateTime to, bool selectedDraft);
    }
}
