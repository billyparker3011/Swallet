using HnMicro.Core.Scopes;
using Lottery.Core.Partners.Models.Allbet;
using Lottery.Data.Entities.Partners.Casino;

namespace Lottery.Core.Services.Partners.CA
{
    public interface ICasinoPlayerMappingService : IScopedDependency
    {
        Task<CasinoPlayerMapping> FindPlayerMappingAsync(long id);

        Task<CasinoPlayerMapping> FindPlayerMappingByPlayerIdAsync(long playerId);

        Task<IEnumerable<CasinoPlayerMapping>> GetPlayerMappingsAsync(string username);

        Task<IEnumerable<CasinoPlayerMapping>> GetAllPlayerMappingsAsync();

        Task CreatePlayerMappingAsync(CreateCasinoPlayerMappingModel model);

        Task UpdatePlayerMappingAsync(UpdateCasinoPlayerMappingModel model);

        Task DeletePlayerMappingAsync(long id);
    }
}
