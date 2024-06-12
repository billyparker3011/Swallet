using HnMicro.Framework.Models;
using Lottery.Core.Models.BetKind;
using Lottery.Core.Models.Channel;
using Lottery.Core.Models.Client;

namespace Lottery.Core.Models.Ticket.Process;

public class ProcessValidationTicketModel
{
    public ClientInformation ClientInformation { get; set; }
    public ClientPlayerModel Player { get; set; }
    public BetKindModel BetKind { get; set; }
    public Data.Entities.Match Match { get; set; }
    public ChannelModel Channel { get; set; }
    public TicketMetadataModel Metadata { get; set; }
}