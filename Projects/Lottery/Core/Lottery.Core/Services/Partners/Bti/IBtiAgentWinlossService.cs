using HnMicro.Core.Scopes;
using Lottery.Core.Partners.Models.Bti;

namespace Lottery.Core.Services.Partners.Bti
{
    public interface IBtiAgentWinlossService : IScopedDependency
    {
        Task<GetBtiAgentWinLossSummaryResultModel> GetBtiAgentWinloss(long? agentId, DateTime from, DateTime to);
    }
}
