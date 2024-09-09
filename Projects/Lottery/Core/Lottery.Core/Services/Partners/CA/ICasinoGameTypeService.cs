using HnMicro.Core.Scopes;
using Lottery.Data.Entities.Partners.Casino;

namespace Lottery.Core.Services.Partners.CA
{
    public interface ICasinoGameTypeService : IScopedDependency
    {
        Task<IEnumerable<CasinoGameType>> GetGameTypesAsync(string category);

        Task<IEnumerable<CasinoGameType>> GetAllGameTypesAsync();
    }
}
