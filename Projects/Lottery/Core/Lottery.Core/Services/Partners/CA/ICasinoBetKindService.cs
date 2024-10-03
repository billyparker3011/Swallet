using HnMicro.Core.Scopes;
using Lottery.Core.Partners.Models.Allbet;
using Lottery.Data.Entities.Partners.Casino;

namespace Lottery.Core.Services.Partners.CA
{
    public interface ICasinoBetKindService : IScopedDependency
    {
        Task<CasinoBetKind> FindBetKindAsync(int id);

        Task<IEnumerable<CasinoBetKind>> GetBetKindsAsync(string name);

        Task<IEnumerable<CasinoBetKind>> GetAllBetKindsAsync();

        Task CreateBetKindAsync(CreateCasinoBetKindModel model);

        Task UpdateBetKindAsync(UpdateCasinoBetKindModel model);

        Task DeleteBetKindAsync(int id);
    }
}
