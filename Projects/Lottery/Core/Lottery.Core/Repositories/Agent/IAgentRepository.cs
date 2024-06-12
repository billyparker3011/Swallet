using HnMicro.Modules.EntityFrameworkCore.Repositories;
using Lottery.Core.Enums;
using Lottery.Data;

namespace Lottery.Core.Repositories.Agent
{
    public interface IAgentRepository : IEntityFrameworkCoreRepository<long, Data.Entities.Agent, LotteryContext>
    {
        Task<Data.Entities.Agent> FindByUsername(string username);
        Task<Data.Entities.Agent> FindByUsernamePassword(string username, string password);
        Task<decimal> SumAllCreditByTypeIdAsync(long typeId, Role role);
        Task<bool> CheckExistAgent(string username);
        Task<int> CountAgentByState(List<long> typeIds, Role role, UserState state);
    }
}
