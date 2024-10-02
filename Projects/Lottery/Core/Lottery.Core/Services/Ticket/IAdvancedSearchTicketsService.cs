using HnMicro.Core.Scopes;
using Lottery.Core.Models.Ticket;

namespace Lottery.Core.Services.Ticket;

public interface IAdvancedSearchTicketsService : IScopedDependency
{
    Task RefundRejectTickets(List<long> ticketIds);
    Task RefundRejectTicketsByNumbers(List<long> ticketIds, List<int> numbers);
    Task<AdvancedSearchTicketsResultModel> Search(AdvancedSearchTicketsModel model);
}
