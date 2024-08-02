using Lottery.Core.Models.BetKind;
using Lottery.Core.Models.Channel;

namespace Lottery.Core.Models.Ticket.Process;

public class ProcessValidationTicketDetailV2Model
{
    public BetKindModel BetKind { get; set; }
    public ChannelModel Channel { get; set; }
    public TicketMetadataModel Metadata { get; set; }
}