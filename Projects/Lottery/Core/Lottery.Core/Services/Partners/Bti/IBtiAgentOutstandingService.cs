using HnMicro.Core.Scopes;
using Lottery.Core.Partners.Models.Bti;

namespace Lottery.Core.Services.Partners.Bti
{
    public interface IBtiAgentOutstandingService : IScopedDependency
    {
        Task<BtiAgentOutstandingResultModel> GetBtiAgentOutstanding(GetBtiAgentOutstandingModel model);
    }
}
