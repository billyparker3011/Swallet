using HnMicro.Core.Scopes;
using Lottery.Core.Models.Ticket.Process;

namespace Lottery.Core.Services.Ticket;

public interface IProcessMixedService : IScopedDependency
{
    Task<List<Data.Entities.Ticket>> Process(ProcessMixedTicketModel model, ProcessValidationTicketModel processValidation);
    Task<List<Data.Entities.Ticket>> ProcessV2(ProcessMixedTicketV2Model model, ProcessValidationTicketV2Model processValidation, ProcessValidationTicketDetailV2Model itemModel);
}
