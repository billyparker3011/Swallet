using HnMicro.Core.Scopes;
using Lottery.Core.Models.CockFight.GetCockFightAgentOutstanding;

namespace Lottery.Core.Services.CockFight
{
    public interface ICockFightAgentOutstandingService : IScopedDependency 
    {
        Task<GetCockFightAgentOutstandingResult> GetCockFightAgentOutstanding(GetCockFightAgentOutstandingModel model);
    }
}
