using HnMicro.Core.Scopes;
using Lottery.Core.Partners.Models.Bti;

namespace Lottery.Core.Services.Partners.Bti
{
    public interface IBtiBetKindService : IScopedDependency
    {
        Task<BtiBetKindModel> FindAsync(long id);

        Task<IEnumerable<BtiBetKindModel>> GetsAsync(string name);

        Task<IEnumerable<BtiBetKindModel>> GetAllAsync();

        Task CreateAsync(BtiBetKindModel model);

        Task UpdateAsync(BtiBetKindModel model);

        Task DeleteAsync(long id);
    }
}
