using HnMicro.Framework.Models;
using Lottery.Core.Models.Client;
using Lottery.Core.Models.Match;

namespace Lottery.Core.Models.Ticket.Process;

public class ProcessValidationTicketV2Model
{
    public ClientInformation ClientInformation { get; set; }
    public ClientPlayerModel Player { get; set; }
    public MatchModel Match { get; set; }
    public List<ProcessValidationTicketDetailV2Model> Details { get; set; }
}
