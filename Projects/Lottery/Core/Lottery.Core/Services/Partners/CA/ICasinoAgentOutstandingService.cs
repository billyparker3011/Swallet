using HnMicro.Core.Scopes;
using Lottery.Core.Partners.Models.Allbet;

namespace Lottery.Core.Services.Partners.CA
{
    public interface ICasinoAgentOutstandingService : IScopedDependency
    {
        Task<CasinoAgentOutstandingResultModel> GetCasinoAgentOutstanding(GetCasinoAgentOutstandingModel model);
    }
}
