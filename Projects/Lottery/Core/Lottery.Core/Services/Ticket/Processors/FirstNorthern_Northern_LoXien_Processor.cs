using HnMicro.Core.Helpers;
using Lottery.Core.Helpers;
using Lottery.Core.Models.Ticket;
using Lottery.Core.Models.Ticket.Process;

namespace Lottery.Core.Services.Ticket.Processors;

public class FirstNorthern_Northern_LoXien_Processor : AbstractBetKindProcessor
{
    private readonly Dictionary<int, int> _subBetKindIds = new()
    {
        { Enums.BetKind.FirstNorthern_Northern_Xien2.ToInt(), 2 },
        { Enums.BetKind.FirstNorthern_Northern_Xien3.ToInt(), 3 },
        { Enums.BetKind.FirstNorthern_Northern_Xien4.ToInt(), 4 }
    };

    public FirstNorthern_Northern_LoXien_Processor(IServiceProvider serviceProvider) : base(serviceProvider)
    {
    }

    public override int BetKindId { get; set; } = Enums.BetKind.FirstNorthern_Northern_LoXien.ToInt();

    public override bool EnableStats()
    {
        return true;
    }

    public override Dictionary<int, int> GetSubBetKindIds()
    {
        return _subBetKindIds;
    }

    public override int ValidMixed(ProcessMixedTicketModel model, TicketMetadataModel metadata)
    {
        return metadata.IsLive ? ErrorCodeHelper.ProcessTicket.NotAccepted : 0;
    }

    public override int ValidMixedV2(ProcessMixedTicketV2Model model, TicketMetadataModel metadata)
    {
        return metadata.IsLive ? ErrorCodeHelper.ProcessTicket.NotAccepted : 0;
    }
}
