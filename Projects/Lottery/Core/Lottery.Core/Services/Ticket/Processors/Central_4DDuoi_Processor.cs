using HnMicro.Core.Helpers;
using HnMicro.Framework.Exceptions;
using Lottery.Core.Helpers;
using Lottery.Core.Models.Ticket.Process;

namespace Lottery.Core.Services.Ticket.Processors;

public class Central_4DDuoi_Processor : AbstractBetKindProcessor
{
    public Central_4DDuoi_Processor(IServiceProvider serviceProvider) : base(serviceProvider)
    {
    }

    public override int BetKindId { get; set; } = Enums.BetKind.Central_4DDuoi.ToInt();

    public override int ValidV2(ProcessTicketV2Model model, List<ProcessValidationTicketDetailV2Model> metadata)
    {
        var metadataItem = metadata.FirstOrDefault(f => f.BetKind != null && f.BetKind.Id == BetKindId) ?? throw new NotFoundException();
        if (metadataItem.Metadata == null) throw new NotFoundException();
        if (metadataItem.Metadata.IsLive) return ErrorCodeHelper.ProcessTicket.NotAccepted;
        var betKindDetail = model.Details.FirstOrDefault(f => f.BetKindId == BetKindId) ?? throw new NotFoundException();
        return betKindDetail.Numbers.Count > NoOfSelectedNumbersExceed ? ErrorCodeHelper.ProcessTicket.NoOfSelectedNumbersExceed512 : 0;
    }
}
