using HnMicro.Core.Helpers;

namespace Lottery.Core.Services.Ticket.Processors;

public class Southern_Xien2_Processor : AbstractBetKindProcessor
{
    public Southern_Xien2_Processor(IServiceProvider serviceProvider) : base(serviceProvider)
    {
    }

    public override int BetKindId { get; set; } = Enums.BetKind.Southern_Xien2.ToInt();
}
