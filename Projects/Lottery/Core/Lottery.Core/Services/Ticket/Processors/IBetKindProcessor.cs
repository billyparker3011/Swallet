﻿using Lottery.Core.Models.BetKind;
using Lottery.Core.Models.MatchResult;
using Lottery.Core.Models.Ticket;
using Lottery.Core.Models.Ticket.Process;

namespace Lottery.Core.Services.Ticket.Processors;

public interface IBetKindProcessor
{
    int BetKindId { get; set; }

    CompletedTicketResultModel Completed(CompletedTicketModel ticket, List<PrizeMatchResultModel> result);
    bool EnableStats();
    decimal GetPayoutByNumber(BetKindModel betKind, decimal point, decimal oddsValue, ProcessPayoutMetadataModel metadata = null);
    Dictionary<int, int> GetSubBetKindIds();
    int Valid(ProcessTicketModel model, TicketMetadataModel metadata);
    int ValidMixed(ProcessMixedTicketModel model, TicketMetadataModel metadata);
}
