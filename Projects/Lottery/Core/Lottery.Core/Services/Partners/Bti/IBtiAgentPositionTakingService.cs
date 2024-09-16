using HnMicro.Core.Scopes;
using Lottery.Core.Partners.Models.Bti;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Lottery.Core.Services.Partners.Bti
{
    public interface IBtiAgentPositionTakingService : IScopedDependency
    {
        Task<BtiAgentPositionTakingModel> FindAsync(long id);

        Task<IEnumerable<BtiAgentPositionTakingModel>> GetsAsync(long agentId);

        Task<IEnumerable<BtiAgentPositionTakingModel>> GetAllAsync();

        Task CreateAsync(BtiAgentPositionTakingModel model);

        Task UpdateAsync(BtiAgentPositionTakingModel model);

        Task DeleteAsync(long id);
    }
}
