using HnMicro.Core.Helpers;
using Lottery.Core.Enums;
using Lottery.Core.Helpers;
using Lottery.Core.Models.BetKind;
using Lottery.Core.Models.MatchResult;
using Lottery.Core.Models.Ticket;
using Lottery.Core.Models.Ticket.Process;

namespace Lottery.Core.Services.Ticket.Processors;

public class FirstNorthern_Northern_DeTruot_Processor : AbstractBetKindProcessor
{
    private const int _prize = 9;
    private const int _nOOfNumbers = 10;

    public override int BetKindId { get; set; } = Enums.BetKind.FirstNorthern_Northern_DeTruot.ToInt();

    public override int Valid(ProcessTicketModel model, TicketMetadataModel metadata)
    {
        if (model.Numbers.Count < _nOOfNumbers) return ErrorCodeHelper.ProcessTicket.FirstNorthern_Northern_DeTruot_MustChooseAtLeast10;
        return metadata.IsLive ? ErrorCodeHelper.ProcessTicket.NotAccepted : 0;
    }

    public override decimal GetPayoutByNumber(BetKindModel betKind, decimal point, decimal oddsValue, ProcessPayoutMetadataModel metadata = null)
    {
        return (oddsValue - betKind.Award) * point;
    }

    public override decimal? GetPlayerOdds(Dictionary<int, decimal> payoutByNumbers)
    {
        foreach (var item in payoutByNumbers) return item.Value;
        return base.GetPlayerOdds(payoutByNumbers);
    }

    public override CompletedTicketResultModel Completed(CompletedTicketModel ticket, List<PrizeMatchResultModel> result)
    {
        var rs = result.FirstOrDefault(f => f.Prize == _prize);
        if (rs == null) return null;
        var latestRs = rs.Results.FirstOrDefault();
        if (latestRs == null) return null;
        if (!latestRs.Result.GetEndOfResult(out string val)) return null;
        if (ticket.Children.Count == 0) return null;

        var dataResult = new CompletedTicketResultModel
        {
            TicketId = ticket.TicketId
        };

        var totalPlayerWinLose = 0m;
        var totalAgentWinLose = 0m;
        var totalAgentCommission = 0m;
        var totalMasterWinLose = 0m;
        var totalMasterCommission = 0m;
        var totalSupermasterWinLose = 0m;
        var totalSupermasterCommission = 0m;
        var totalCompanyWinLose = 0m;
        foreach (var item in ticket.Children)
        {
            var playerWinlose = item.ChoosenNumbers.Equals(val)
                                    ? -1 * item.PlayerPayout
                                    : item.Stake * ticket.RewardRate.Value;
            var agentWinlose = -1 * playerWinlose * item.AgentPt;
            var agentCommission = (item.PlayerOdds ?? 0m - item.AgentOdds ?? 0m) * item.Stake;
            var masterWinlose = -1 * (item.MasterPt - item.AgentPt) * playerWinlose;
            var masterCommission = (item.AgentOdds ?? 0m - item.MasterOdds ?? 0m) * item.Stake;
            var supermasterWinlose = -1 * (item.SupermasterPt - item.MasterPt) * playerWinlose;
            var supermasterCommission = (item.MasterOdds ?? 0m - item.SupermasterOdds ?? 0m) * item.Stake;
            var companyWinlose = -1 * (1 - item.SupermasterPt) * playerWinlose;

            dataResult.Children.Add(new CompletedChildrenTicketResultModel
            {
                TicketId = item.TicketId,
                State = item.ChoosenNumbers.Equals(val) ? TicketState.Lose : TicketState.Won,
                PlayerWinLoss = playerWinlose,
                AgentWinLoss = agentWinlose,
                AgentCommission = agentCommission,
                MasterWinLoss = masterWinlose,
                MasterCommission = masterCommission,
                SupermasterWinLoss = supermasterWinlose,
                SupermasterCommission = supermasterCommission,
                CompanyWinLoss = companyWinlose
            });

            totalPlayerWinLose += playerWinlose;
            totalAgentWinLose += agentWinlose;
            totalAgentCommission += agentCommission;
            totalMasterWinLose += masterWinlose;
            totalMasterCommission += masterCommission;
            totalSupermasterWinLose += supermasterWinlose;
            totalSupermasterCommission += supermasterCommission;
            totalCompanyWinLose += companyWinlose;
        }

        dataResult.State = totalPlayerWinLose < 0 ? TicketState.Lose : TicketState.Won;
        dataResult.PlayerWinLoss = totalPlayerWinLose;
        dataResult.AgentWinLoss = totalAgentWinLose;
        dataResult.AgentCommission = totalAgentCommission;
        dataResult.MasterWinLoss = totalMasterWinLose;
        dataResult.MasterCommission = totalMasterCommission;
        dataResult.SupermasterWinLoss = totalSupermasterWinLose;
        dataResult.SupermasterCommission = totalSupermasterCommission;
        dataResult.CompanyWinLoss = totalCompanyWinLose;
        return dataResult;
    }

    public override RefundRejectTicketByNumbersResultModel RefundRejectTicketByNumbers(RefundRejectTicketByNumbersModel model)
    {
        var result = new RefundRejectTicketByNumbersResultModel
        {
            Allow = true,
            Ticket = model.Ticket,
            Children = model.Children
        };
        var enableStats = EnableStats();
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

        var noOfWaitingRunningTickets = result.Children.Count(f => !refundRejectTicketState.Contains(f.State.ToInt()));
        if (noOfWaitingRunningTickets < _nOOfNumbers)
        {
            result.Ticket.State = model.Ticket.IsLive.GetRefundRejectStateByIsLive();
            foreach (var item in result.Children)
            {
                if (refundRejectTicketState.Contains(item.State.ToInt())) continue;
                item.State = result.Ticket.State;
                result.DifferentPlayerPayout += item.PlayerPayout;
            }
        }
        else
        {
            result.Ticket.Stake = result.Children.Where(f => !refundRejectTicketState.Contains(f.State.ToInt())).Sum(f => f.Stake);
            result.Ticket.PlayerPayout = result.Children.Where(f => !refundRejectTicketState.Contains(f.State.ToInt())).Sum(f => f.PlayerPayout);
            result.Ticket.AgentPayout = result.Children.Where(f => !refundRejectTicketState.Contains(f.State.ToInt())).Sum(f => f.AgentPayout);
            result.Ticket.MasterPayout = result.Children.Where(f => !refundRejectTicketState.Contains(f.State.ToInt())).Sum(f => f.MasterPayout);
            result.Ticket.SupermasterPayout = result.Children.Where(f => !refundRejectTicketState.Contains(f.State.ToInt())).Sum(f => f.SupermasterPayout);
            result.Ticket.CompanyPayout = result.Children.Where(f => !refundRejectTicketState.Contains(f.State.ToInt())).Sum(f => f.CompanyPayout);
        }
        return result;
    }

    public override RefundRejectTicketResultModel RefundRejectTicket(RefundRejectTicketModel model)
    {
        var result = new RefundRejectTicketResultModel();
        var enableStats = EnableStats();
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
        return result;
    }
}
