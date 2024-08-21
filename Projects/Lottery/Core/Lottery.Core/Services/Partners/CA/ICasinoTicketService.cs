using HnMicro.Core.Scopes;
using Lottery.Core.Partners.Models.Allbet;
using Lottery.Data.Entities.Partners.Casino;

namespace Lottery.Core.Services.Partners.CA
{
    public interface ICasinoTicketService : IScopedDependency
    {
        Task<CasinoTicket> FindAsync(long id);

        Task<CasinoTicket> FindWithIncludeAsync(long id);

        Task<IEnumerable<CasinoTicket>> GetsByPlayerIdAsync(long playerId);

        Task<IEnumerable<CasinoTicket>> GetsByPlayerIdWithIncludeAsync(long playerId);

        Task<IEnumerable<CasinoTicket>> GetsByBookiePlayerIdAsync(string bookiePlayerId);

        Task<IEnumerable<CasinoTicket>> GetsByBookiePlayerIdWithIncludeAsync(string bookiePlayerId);

        Task<IEnumerable<CasinoTicket>> GetAllAsync();

        Task<decimal> ProcessTicketAsync(CasinoTicketModel model, decimal balance);

        Task<decimal> ProcessCancelTicketAsync(CasinoCancelTicketModel model, decimal balance);

        Task CreateCasinoTicketAsync(CasinoTicketModel model);

        Task CreateCasinoCancelTicketAsync(CasinoCancelTicketModel model);
    }
}
