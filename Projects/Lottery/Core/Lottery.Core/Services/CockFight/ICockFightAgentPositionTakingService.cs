using HnMicro.Core.Scopes;
using Lottery.Core.Models.CockFight.GetCockFightAgentPositionTaking;
using Lottery.Core.Models.CockFight.UpdateCockFightAgentPositionTaking;

namespace Lottery.Core.Services.CockFight
{
    public interface ICockFightAgentPositionTakingService : IScopedDependency
    {
        Task<GetCockFightAgentPositionTakingResult> GetCockFightAgentPositionTakingDetail(long agentId);
        Task UpdateCockFightAgentPositionTaking(long agentId, UpdateCockFightAgentPositionTakingModel model);
        Task<GetCockFightAgentPositionTakingResult> GetDefaultCockFightCompanyPositionTaking();
        Task UpdateDefaultCockFightCompanyPositionTaking(UpdateCockFightAgentPositionTakingModel model);
    }
}
