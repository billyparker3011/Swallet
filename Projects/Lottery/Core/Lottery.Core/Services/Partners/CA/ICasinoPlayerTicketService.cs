using HnMicro.Core.Scopes;
using Lottery.Core.Partners.Models.Allbet;

namespace Lottery.Core.Services.Partners.CA
{
    public interface ICasinoPlayerTicketService : IScopedDependency
    {
        Task<List<CasinoBetTableTicketModel>> GetCasinoPlayerOuts(long playerId);
        Task<List<CasinoBetTableTicketModel>> GetCasinoPlayerWinloseDetail(GetCasinoPlayerWinlossDetailModel model);
    }
}
