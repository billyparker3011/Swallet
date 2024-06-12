using HnMicro.Core.Scopes;
using Lottery.Core.Models.Agent.GetAgentOuts;
using Lottery.Core.Models.Agent.GetAgentOutstanding;

namespace Lottery.Core.Services.Agent
{
    public interface IAgentOutstandingService: IScopedDependency
    {
        Task<GetAgentOutstandingResult> GetAgentOutstandings(GetAgentOutstandingModel model);
    }
}
