using HnMicro.Core.Scopes;
using Lottery.Core.Models.Agent.GetAgentOuts;
using Lottery.Core.Models.Agent.GetAgentOutstanding;
using Lottery.Core.Models.BroadCaster.GetBroadCasterOutstanding;

namespace Lottery.Core.Services.Agent
{
    public interface IAgentOutstandingService: IScopedDependency
    {
        Task<GetAgentOutstandingResult> GetAgentOutstandings(GetAgentOutstandingModel model);
    }
}
