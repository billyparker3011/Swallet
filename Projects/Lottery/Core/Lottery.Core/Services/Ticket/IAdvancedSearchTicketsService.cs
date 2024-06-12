using HnMicro.Core.Scopes;
using Lottery.Core.Models.Ticket;

namespace Lottery.Core.Services.Ticket;

public interface IAdvancedSearchTicketsService : IScopedDependency
{
    Task RefundRejectTickets(List<long> ticketIds);
    Task<AdvancedSearchTicketsResultModel> Search(AdvancedSearchTicketsModel model);
}
