using HnMicro.Core.Scopes;
using Lottery.Core.Models.Ticket.Process;

namespace Lottery.Core.Services.Ticket;

public interface IProcessLiveService : IScopedDependency
{
    Task<(Data.Entities.Ticket, List<Data.Entities.Ticket>)> Process(ProcessTicketModel model, ProcessValidationTicketModel processValidation);
    Task<(Data.Entities.Ticket, List<Data.Entities.Ticket>)> ProcessV2(ProcessTicketDetailV2Model model, ProcessValidationTicketV2Model processValidation);
}
