using HnMicro.Core.Scopes;
using Lottery.Core.Models.Ticket.Process;

namespace Lottery.Core.Services.Ticket;

public interface IProcessNoneLiveService : IScopedDependency
{
    Task<(Data.Entities.Ticket, List<Data.Entities.Ticket>)> Process(ProcessTicketModel model, ProcessValidationTicketModel processValidation);
    Task<(Data.Entities.Ticket, List<Data.Entities.Ticket>)> ProcessV2(ProcessTicketDetailV2Model itemModel, ProcessValidationTicketV2Model processValidation);
}
