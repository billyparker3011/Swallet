using HnMicro.Core.Helpers;
using HnMicro.Framework.Exceptions;
using Lottery.Core.Models.BetKind;
using Lottery.Core.Models.Ticket;
using Lottery.Core.Models.Ticket.Process;
using Lottery.Core.Services.Ticket.Processors;

namespace Lottery.Core.Services.Ticket;

public class TicketProcessor : ITicketProcessor
{
    private readonly List<IBetKindProcessor> _handlers = new();
    private readonly IServiceProvider _serviceProvider;

    public TicketProcessor(IServiceProvider serviceProvider)
    {
        _serviceProvider = serviceProvider;

        LoadBetKindProcessors();
    }

    public RefundRejectTicketByNumbersResultModel RefundRejectTicketByNumbers(int betKindId, RefundRejectTicketByNumbersModel model)
    {
        var handler = _handlers.FirstOrDefault(f => f.BetKindId == betKindId);
        if (handler == null) return null;
        return handler.RefundRejectTicketByNumbers(model);
    }

    public RefundRejectTicketResultModel RefundRejectTicket(int betKindId, RefundRejectTicketModel model)
    {
        var handler = _handlers.FirstOrDefault(f => f.BetKindId == betKindId);
        if (handler == null) return null;
        return handler.RefundRejectTicket(model);
    }

    public CompletedTicketResultModel CompletedTicket(int betKindId, CompletedTicketModel ticket, List<Models.MatchResult.PrizeMatchResultModel> result)
    {
        var handler = _handlers.FirstOrDefault(f => f.BetKindId == betKindId);
        if (handler == null) return null;
        return handler.Completed(ticket, result);
    }

    public bool EnableStats(int betKindId)
    {
        var handler = _handlers.FirstOrDefault(f => f.BetKindId == betKindId);
        if (handler == null) return false;
        return handler.EnableStats();
    }

    public decimal GetPayoutByNumber(BetKindModel betKind, int point, decimal oddsValue, ProcessPayoutMetadataModel metadata = null)
    {
        var handler = _handlers.FirstOrDefault(f => f.BetKindId == betKind.Id);
        if (handler == null) return 0m;
        return handler.GetPayoutByNumber(betKind, point, oddsValue, metadata);
    }

    public decimal? GetPlayerOdds(int betKindId, Dictionary<int, decimal> oddsValueByNumbers)
    {
        var handler = _handlers.FirstOrDefault(f => f.BetKindId == betKindId);
        if (handler == null) return null;
        return handler.GetPlayerOdds(oddsValueByNumbers);
    }

    public Dictionary<int, int> GetSubBetKindIds(int betKindId)
    {
        var handler = _handlers.FirstOrDefault(f => f.BetKindId == betKindId);
        if (handler == null) return new Dictionary<int, int>();
        return handler.GetSubBetKindIds();
    }

    public int Valid(ProcessTicketModel model, TicketMetadataModel metadata)
    {
        var handler = _handlers.FirstOrDefault(f => f.BetKindId == model.BetKindId);
        return handler == null ? throw new HnMicroException() : handler.Valid(model, metadata);
    }

    public int ValidMixed(ProcessMixedTicketModel model, TicketMetadataModel metadata)
    {
        var handler = _handlers.FirstOrDefault(f => f.BetKindId == model.BetKindId);
        return handler == null ? throw new HnMicroException() : handler.ValidMixed(model, metadata);
    }

    private void LoadBetKindProcessors()
    {
        var types = typeof(IBetKindProcessor).GetDerivedClass().ToList();
        foreach (var item in types) _handlers.Add(Activator.CreateInstance(item, _serviceProvider) as IBetKindProcessor);
    }

    public decimal GetRealPayoutForCompany(decimal playerPayout, decimal supermasterPt)
    {
        return (1 - supermasterPt) * playerPayout;
    }

    public int ValidV2(ProcessTicketV2Model model, int betKindId, List<ProcessValidationTicketDetailV2Model> metadata)
    {
        var handler = _handlers.FirstOrDefault(f => f.BetKindId == betKindId);
        return handler == null ? throw new HnMicroException() : handler.ValidV2(model, metadata);
    }

    public int ValidMixedV2(ProcessMixedTicketV2Model model, TicketMetadataModel metadata)
    {
        var handler = _handlers.FirstOrDefault(f => f.BetKindId == model.BetKindId);
        return handler == null ? throw new HnMicroException() : handler.ValidMixedV2(model, metadata);
    }
}
