using HnMicro.Core.Helpers;
using Lottery.Core.Helpers;
using Lottery.Core.Models.BetKind;
using Lottery.Core.Models.MatchResult;
using Lottery.Core.Models.Ticket;
using Lottery.Core.Models.Ticket.Process;

namespace Lottery.Core.Services.Ticket.Processors;

public abstract class AbstractBetKindProcessor : IBetKindProcessor
{
    protected const int NoOfSelectedNumbersExceed = 512;
    protected IServiceProvider ServiceProvider;

    protected AbstractBetKindProcessor(IServiceProvider serviceProvider)
    {
        ServiceProvider = serviceProvider;
    }

    protected List<string> SplitChooseNumbers(string chooseNumbers)
    {
        chooseNumbers = string.IsNullOrEmpty(chooseNumbers) ? string.Empty : chooseNumbers.Trim();
        if (string.IsNullOrEmpty(chooseNumbers)) return new List<string>();
        return chooseNumbers.Split(new[] { "," }, StringSplitOptions.TrimEntries).ToList();
    }

    public abstract int BetKindId { get; set; }

    public virtual RefundRejectTicketByNumbersResultModel RefundRejectTicketByNumbers(RefundRejectTicketByNumbersModel model)
    {
        var result = new RefundRejectTicketByNumbersResultModel
        {
            Allow = true,
            Ticket = model.Ticket,
            Children = model.Children
        };
        var enableStats = EnableStats();
        if (model.Children.Count == 0)
        {
            if (model.RefundRejectNumbers.Contains(model.Ticket.ChoosenNumbers))
            {
                result.Ticket.State = model.Ticket.IsLive.GetRefundRejectStateByIsLive();
                result.DifferentPlayerPayout += model.Ticket.PlayerPayout;
                result.OutsByNumbers[int.Parse(model.Ticket.ChoosenNumbers)] = model.Ticket.PlayerPayout;
                result.PointsByNumbers[int.Parse(model.Ticket.ChoosenNumbers)] = model.Ticket.Stake;
                if (enableStats)
                {
                    result.OutsByBetKind[int.Parse(model.Ticket.ChoosenNumbers)] = model.Ticket.PlayerPayout;
                    result.PointsByBetKind[int.Parse(model.Ticket.ChoosenNumbers)] = model.Ticket.Stake;
                }
            }
        }
        else
        {
            var refundRejectTicketState = CommonHelper.RefundRejectTicketState();
            foreach (var item in result.Children)
            {
                if (model.RefundRejectNumbers.Contains(item.ChoosenNumbers) && !refundRejectTicketState.Contains(item.State.ToInt()))
                {
                    item.State = item.IsLive.GetRefundRejectStateByIsLive();
                    result.DifferentPlayerPayout += item.PlayerPayout;
                    result.OutsByNumbers[int.Parse(item.ChoosenNumbers)] = item.PlayerPayout;
                    result.PointsByNumbers[int.Parse(item.ChoosenNumbers)] = item.Stake;
                    if (enableStats)
                    {
                        result.OutsByBetKind[int.Parse(item.ChoosenNumbers)] = item.PlayerPayout;
                        result.PointsByBetKind[int.Parse(item.ChoosenNumbers)] = item.Stake;
                    }
                }
            }

            var noOfChildrenTickets = result.Children.Count();
            var noOfRefundRejectTickets = result.Children.Count(f => refundRejectTicketState.Contains(f.State.ToInt()));
            if (noOfChildrenTickets == noOfRefundRejectTickets) result.Ticket.State = model.Ticket.IsLive.GetRefundRejectStateByIsLive();
            else
            {
                result.Ticket.Stake = result.Children.Where(f => !refundRejectTicketState.Contains(f.State.ToInt())).Sum(f => f.Stake);
                result.Ticket.PlayerPayout = result.Children.Where(f => !refundRejectTicketState.Contains(f.State.ToInt())).Sum(f => f.PlayerPayout);
                result.Ticket.AgentPayout = result.Children.Where(f => !refundRejectTicketState.Contains(f.State.ToInt())).Sum(f => f.AgentPayout);
                result.Ticket.MasterPayout = result.Children.Where(f => !refundRejectTicketState.Contains(f.State.ToInt())).Sum(f => f.MasterPayout);
                result.Ticket.SupermasterPayout = result.Children.Where(f => !refundRejectTicketState.Contains(f.State.ToInt())).Sum(f => f.SupermasterPayout);
                result.Ticket.CompanyPayout = result.Children.Where(f => !refundRejectTicketState.Contains(f.State.ToInt())).Sum(f => f.CompanyPayout);
            }
        }
        return result;
    }

    public virtual RefundRejectTicketResultModel RefundRejectTicket(RefundRejectTicketModel model)
    {
        var result = new RefundRejectTicketResultModel();
        var enableStats = EnableStats();
        if (model.Children.Count == 0)
        {
            result.DifferentPlayerPayout += model.Ticket.PlayerPayout;
            result.OutsByNumbers[int.Parse(model.Ticket.ChoosenNumbers)] = model.Ticket.PlayerPayout;
            result.PointsByNumbers[int.Parse(model.Ticket.ChoosenNumbers)] = model.Ticket.Stake;
            if (enableStats)
            {
                result.OutsByBetKind[int.Parse(model.Ticket.ChoosenNumbers)] = model.Ticket.PlayerPayout;
                result.PointsByBetKind[int.Parse(model.Ticket.ChoosenNumbers)] = model.Ticket.Stake;
            }
        }
        else
        {
            var refundRejectTicketState = CommonHelper.RefundRejectTicketState();
            foreach (var item in model.Children)
            {
                if (!refundRejectTicketState.Contains(item.State.ToInt()))
                {
                    result.DifferentPlayerPayout += item.PlayerPayout;
                    result.OutsByNumbers[int.Parse(item.ChoosenNumbers)] = item.PlayerPayout;
                    result.PointsByNumbers[int.Parse(item.ChoosenNumbers)] = item.Stake;
                    if (enableStats)
                    {
                        result.OutsByBetKind[int.Parse(item.ChoosenNumbers)] = item.PlayerPayout;
                        result.PointsByBetKind[int.Parse(item.ChoosenNumbers)] = item.Stake;
                    }
                }
            }
        }
        return result;
    }

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

    public virtual decimal? GetPlayerOdds(Dictionary<int, decimal> oddsValueByNumbers)
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

    public virtual int ValidV2(ProcessTicketV2Model model, List<ProcessValidationTicketDetailV2Model> metadata)
    {
        return 0;
    }
}
