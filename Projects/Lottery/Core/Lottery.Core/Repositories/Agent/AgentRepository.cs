using HnMicro.Modules.EntityFrameworkCore.Repositories;
using Lottery.Core.Enums;
using Lottery.Data;
using Microsoft.EntityFrameworkCore;

namespace Lottery.Core.Repositories.Agent
{
    public class AgentRepository : EntityFrameworkCoreRepository<long, Data.Entities.Agent, LotteryContext>, IAgentRepository
    {
        public AgentRepository(LotteryContext context) : base(context)
        {
        }

        public async Task<bool> CheckExistAgent(string username)
        {
            return await FindQueryBy(x => x.Username.ToLower() == username.ToLower()).AnyAsync();
        }

        public async Task<int> CountAgentByState(List<long> typeIds, Role role, UserState state)
        {
            switch (role)
            {
                case Role.Company:
                    return await FindQueryBy(x => typeIds.Contains(x.SupermasterId) && typeIds.Contains(x.MasterId) && x.RoleId == (int)role + 1 && x.State == (int)state).CountAsync();
                case Role.Supermaster:
                    return await FindQueryBy(x => typeIds.Contains(x.SupermasterId) && x.RoleId == (int)role + 1 && x.State == (int)state).CountAsync();
                case Role.Master:
                    return await FindQueryBy(x => typeIds.Contains(x.MasterId) && x.RoleId == (int)role + 1 && x.State == (int)state).CountAsync();
                default:
                    return 0;
            }
        }

        public async Task<Data.Entities.Agent> FindByUsername(string username)
        {
            return await FindQueryBy(f => f.Username.ToLower().Equals(username.ToLower())).Include(f => f.AgentSession).FirstOrDefaultAsync();
        }

        public async Task<Data.Entities.Agent> FindByUsernamePassword(string username, string password)
        {
            return await FindQueryBy(f => f.Username.ToLower().Equals(username.ToLower()) && f.Password.Equals(password)).Include(f => f.AgentSession).FirstOrDefaultAsync();
        }

        public async Task<decimal> SumAllCreditByTypeIdAsync(long typeId, Role role)
        {
            switch (role)
            {
                case Role.Company:
                    return await FindQueryBy(x => x.RoleId == (int)role + 1 && x.ParentId == 0L).SumAsync(x => x.Credit);
                case Role.Supermaster:
                    return await FindQueryBy(x => x.SupermasterId == typeId && x.RoleId == (int)role + 1).SumAsync(x => x.Credit);
                case Role.Master:
                    return await FindQueryBy(x => x.MasterId == typeId && x.RoleId == (int)role + 1).SumAsync(x => x.Credit);
                default:
                    return decimal.Zero;
            };
        }
    }
}
