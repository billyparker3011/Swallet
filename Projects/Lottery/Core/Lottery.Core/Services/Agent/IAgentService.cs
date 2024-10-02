using HnMicro.Core.Scopes;
using Lottery.Core.Dtos.Agent;
using Lottery.Core.Models.Agent.CreateAgent;
using Lottery.Core.Models.Agent.CreateSubAgent;
using Lottery.Core.Models.Agent.GetAgentCreditBalance;
using Lottery.Core.Models.Agent.GetAgentCreditInfo;
using Lottery.Core.Models.Agent.GetAgentDashBoard;
using Lottery.Core.Models.Agent.GetAgents;
using Lottery.Core.Models.Agent.GetAgentWinLossSummary;
using Lottery.Core.Models.Agent.GetCreditBalanceDetailPopup;
using Lottery.Core.Models.Agent.GetSubAgents;
using Lottery.Core.Models.Agent.UpdateAgent;
using Lottery.Core.Models.Agent.UpdateAgentCreditBalance;

namespace Lottery.Core.Services.Agent
{
    public interface IAgentService : IScopedDependency
    {
        Task CreateAgent(CreateAgentModel model);
        Task<GetAgentsResult> GetAgents(GetAgentsModel model);
        Task UpdateAgent(UpdateAgentModel updateModel);
        Task<bool> CheckExistAgent(string username, bool isSubAgent);
        Task<GetAgentCreditInfoResult> GetAgentCreditInfo(long? agentId);
        Task CreateSubAgent(CreateSubAgentModel model);
        Task<string> GetSuggestionAgentIdentifier(bool isSubAgent);
        Task<GetAgentDashBoardResult> GetAgentDashBoard();
        Task<List<AgentSumarryInfo>> GetNewPlayersOfTheMonth();
        Task<List<AgentSumarryInfo>> GetHighestTurnOverPlayersOfTheMonth();
        Task<List<AgentSumarryInfo>> GetTopPlayersOfTheMonth();
        Task<List<AgentBreadCrumbsDto>> GetBreadCrumbs(long? agentId, int? roleId);
        Task<GetSubAgentsResult> GetSubAgents();
        Task<GetAgentWinLossSummaryResult> GetAgentWinLossSummary(long? agentId, DateTime from, DateTime to, bool selectedDraft);
        Task UpdateAgentBetSetting(long agentId, List<AgentBetSettingDto> updateItems);
        Task UpdateAgentPositionTaking(long agentId, List<AgentPositionTakingDto> updateItems);
        Task<GetAgentCreditBalanceResult> GetAgentCreditBalances(GetAgentCreditBalanceModel model);
        Task<GetCreditBalanceDetailPopupResult> GetCreditBalanceDetailPopup(long agentId);
        Task UpdateAgentCreditBalance(UpdateAgentCreditBalanceModel updateItem);
        Task<List<SearchAgentDto>> SearchAgent(string searchTerm);
    }
}
