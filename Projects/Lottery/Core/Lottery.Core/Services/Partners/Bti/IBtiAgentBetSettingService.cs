using HnMicro.Core.Scopes;
using HnMicro.Framework.Exceptions;
using Lottery.Core.Partners.Models.Bti;
using Lottery.Core.Repositories.Bti;
using Lottery.Core.UnitOfWorks;
using Lottery.Data.Entities.Partners.Bti;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Lottery.Core.Services.Partners.Bti
{
    public interface IBtiAgentBetSettingService : IScopedDependency
    {
        Task<BtiAgentBetSettingModel> FindAsync(long id);

        Task<IEnumerable<BtiAgentBetSettingModel>> GetsAsync(long agentId);

        Task<IEnumerable<BtiAgentBetSettingModel>> GetAllAsync();

        Task CreateAsync(BtiAgentBetSettingModel model);

        Task UpdateAsync(BtiAgentBetSettingModel model);

        Task DeleteAsync(long id);
    }
}
