using HnMicro.Core.Helpers;

namespace Lottery.Core.Services.Ticket.Processors;

public class Central_Xien3_Processor : AbstractBetKindProcessor
{
    public Central_Xien3_Processor(IServiceProvider serviceProvider) : base(serviceProvider)
    {
    }

    public override int BetKindId { get; set; } = Enums.BetKind.Central_Xien3.ToInt();
}
