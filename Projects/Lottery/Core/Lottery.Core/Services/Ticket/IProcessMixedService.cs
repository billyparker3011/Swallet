using HnMicro.Core.Scopes;
using Lottery.Core.Models.Ticket.Process;

namespace Lottery.Core.Services.Ticket;

public interface IProcessMixedService : IScopedDependency
{
    Task<List<Data.Entities.Ticket>> Process(ProcessMixedTicketModel model, ProcessValidationTicketModel processValidation);
}
