using Lottery.Core.Models.BetKind;
using Lottery.Core.Models.MatchResult;
using Lottery.Core.Models.Ticket;
using Lottery.Core.Models.Ticket.Process;

namespace Lottery.Core.Services.Ticket.Processors;

public abstract class AbstractBetKindProcessor : IBetKindProcessor
{
    public abstract int BetKindId { get; set; }

    public virtual CompletedTicketResultModel Completed(CompletedTicketModel ticket, List<PrizeMatchResultModel> result)
    {
        return null;
    }

    public virtual bool EnableStats()
    {
        return false;
    }

    public virtual decimal GetPayoutByNumber(BetKindModel betKind, decimal point, decimal oddsValue, ProcessPayoutMetadataModel metadata = null)
    {
        return point * oddsValue;
    }

    public virtual decimal? GetPlayerOdds(Dictionary<int, decimal> payoutByNumbers)
    {
        return null;
    }

    public virtual Dictionary<int, int> GetSubBetKindIds()
    {
        return new Dictionary<int, int>();
    }

    public virtual int Valid(ProcessTicketModel model, TicketMetadataModel metadata)
    {
        return 0;
    }

    public virtual int ValidMixed(ProcessMixedTicketModel model, TicketMetadataModel metadata)
    {
        return 0;
    }
}
