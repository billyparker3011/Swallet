using HnMicro.Core.Scopes;
using Lottery.Core.Partners.Models.Bti;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Lottery.Core.Services.Partners.Bti
{
    public interface IBtiPlayerBetSettingService : IScopedDependency
    {
        Task<BtiPlayerBetSettingModel> FindAsync(long id);

        Task<IEnumerable<BtiPlayerBetSettingModel>> GetsAsync(long agentId);

        Task<IEnumerable<BtiPlayerBetSettingModel>> GetAllAsync();

        Task CreateAsync(BtiPlayerBetSettingModel model);

        Task UpdateAsync(BtiPlayerBetSettingModel model);

        Task DeleteAsync(long id);
    }
}
