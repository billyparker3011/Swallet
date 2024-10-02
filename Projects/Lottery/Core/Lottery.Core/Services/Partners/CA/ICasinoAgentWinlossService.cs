using HnMicro.Core.Scopes;
using Lottery.Core.Partners.Models.Allbet;

namespace Lottery.Core.Services.Partners.CA
{
    public interface ICasinoAgentWinlossService : IScopedDependency
    {
        Task<GetCasinoAgentWinLossSummaryResultModel> GetCasinoAgentWinloss(long? agentId, DateTime from, DateTime to);
    }
}
