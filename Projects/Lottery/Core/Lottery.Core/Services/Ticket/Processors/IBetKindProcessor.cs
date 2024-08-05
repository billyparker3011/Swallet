using Lottery.Core.Models.BetKind;
using Lottery.Core.Models.MatchResult;
using Lottery.Core.Models.Ticket;
using Lottery.Core.Models.Ticket.Process;

namespace Lottery.Core.Services.Ticket.Processors;

public interface IBetKindProcessor
{
    int BetKindId { get; set; }

    RefundRejectTicketByNumbersResultModel RefundRejectTicketByNumbers(RefundRejectTicketByNumbersModel model);
    RefundRejectTicketResultModel RefundRejectTicket(RefundRejectTicketModel model);
    CompletedTicketResultModel Completed(CompletedTicketModel ticket, List<PrizeMatchResultModel> result);
    CompletedTicketResultModel Completed(CompletedTicketModel ticket, Dictionary<int, List<PrizeMatchResultModel>> results);
    bool EnableStats();
    decimal GetPayoutByNumber(BetKindModel betKind, decimal point, decimal oddsValue, ProcessPayoutMetadataModel metadata = null);
    decimal? GetPlayerOdds(Dictionary<int, decimal> oddsValueByNumbers);
    Dictionary<int, int> GetSubBetKindIds();
    int Valid(ProcessTicketModel model, TicketMetadataModel metadata);
    int ValidV2(ProcessTicketV2Model model, List<ProcessValidationTicketDetailV2Model> metadata);
    int ValidMixed(ProcessMixedTicketModel model, TicketMetadataModel metadata);
    int ValidMixedV2(ProcessMixedTicketV2Model model, TicketMetadataModel metadata);
}
