using HnMicro.Core.Scopes;
using Lottery.Core.Models.CockFight.GetCockFightAgentBetKind;

namespace Lottery.Core.Services.CockFight
{
    public interface ICockFightAgentBetKindService : IScopedDependency 
    {
        Task<GetCockFightAgentBetKindModel> GetCockFightAgentBetKind();
        Task UpdateCockFightAgentBetKind(GetCockFightAgentBetKindModel model);
    }

}
