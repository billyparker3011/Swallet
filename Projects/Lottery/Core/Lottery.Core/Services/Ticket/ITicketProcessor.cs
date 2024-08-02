using HnMicro.Core.Scopes;
using Lottery.Core.Models.BetKind;
using Lottery.Core.Models.MatchResult;
using Lottery.Core.Models.Ticket;
using Lottery.Core.Models.Ticket.Process;

namespace Lottery.Core.Services.Ticket;

public interface ITicketProcessor : ISingletonDependency
{
    int Valid(ProcessTicketModel model, TicketMetadataModel metadata);
    int ValidMixed(ProcessMixedTicketModel model, TicketMetadataModel metadata);
    bool EnableStats(int betKindId);
    CompletedTicketResultModel CompletedTicket(int betKindId, CompletedTicketModel ticket, List<PrizeMatchResultModel> result);
    Dictionary<int, int> GetSubBetKindIds(int betKindId);
    decimal GetPayoutByNumber(BetKindModel betKind, int point, decimal oddsValue, ProcessPayoutMetadataModel metadata = null);
    decimal? GetPlayerOdds(int betKindId, Dictionary<int, decimal> oddsValueByNumbers);
    RefundRejectTicketByNumbersResultModel RefundRejectTicketByNumbers(int betKindId, RefundRejectTicketByNumbersModel model);
    RefundRejectTicketResultModel RefundRejectTicket(int betKindId, RefundRejectTicketModel model);
    decimal GetRealPayoutForCompany(decimal playerPayout, decimal supermasterPt);

    int ValidV2(ProcessTicketV2Model model, int betKindId, List<ProcessValidationTicketDetailV2Model> metadata);
}
