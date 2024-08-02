using HnMicro.Core.Helpers;

namespace Lottery.Core.Services.Ticket.Processors;

public class Central_Xien2_Processor : AbstractBetKindProcessor
{
    public Central_Xien2_Processor(IServiceProvider serviceProvider) : base(serviceProvider)
    {
    }

    public override int BetKindId { get; set; } = Enums.BetKind.Central_Xien2.ToInt();
}
