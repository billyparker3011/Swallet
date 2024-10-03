using HnMicro.Core.Scopes;
using Lottery.Core.Models.CockFight.GetCockFightAgentWinloss;

namespace Lottery.Core.Services.CockFight
{
    public interface ICockFightAgentWinlossService : IScopedDependency
    {
        Task<GetCockFightAgentWinLossSummaryResult> GetCockFightAgentWinloss(long? agentId, DateTime from, DateTime to);
    }
}
