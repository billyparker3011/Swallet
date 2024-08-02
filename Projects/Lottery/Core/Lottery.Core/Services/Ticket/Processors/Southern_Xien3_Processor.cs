using HnMicro.Core.Helpers;

namespace Lottery.Core.Services.Ticket.Processors;

public class Southern_Xien3_Processor : AbstractBetKindProcessor
{
    public Southern_Xien3_Processor(IServiceProvider serviceProvider) : base(serviceProvider)
    {
    }

    public override int BetKindId { get; set; } = Enums.BetKind.Southern_Xien3.ToInt();
}
