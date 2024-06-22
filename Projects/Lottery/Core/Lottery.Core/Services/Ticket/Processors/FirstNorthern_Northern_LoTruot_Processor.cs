using HnMicro.Core.Helpers;
using Lottery.Core.Enums;
using Lottery.Core.Helpers;
using Lottery.Core.Models.BetKind;
using Lottery.Core.Models.MatchResult;
using Lottery.Core.Models.Ticket;
using Lottery.Core.Models.Ticket.Process;

namespace Lottery.Core.Services.Ticket.Processors;

public class FirstNorthern_Northern_LoTruot_Processor : AbstractBetKindProcessor
{
    private const int _startedPrize = 2;    //  Exclude Than Tai
    private const int _endPrize = 9;

    public override int BetKindId { get; set; } = Enums.BetKind.FirstNorthern_Northern_LoTruot.ToInt();

    public override int Valid(ProcessTicketModel model, TicketMetadataModel metadata)
    {
        if (model.Numbers.Count < 4) return ErrorCodeHelper.ProcessTicket.FirstNorthern_Northern_LoTruot_MustChooseAtLeast4;
        return metadata.IsLive ? ErrorCodeHelper.ProcessTicket.NotAccepted : 0;
    }

    public override decimal GetPayoutByNumber(BetKindModel betKind, decimal point, decimal oddsValue, ProcessPayoutMetadataModel metadata = null)
    {
        return point * (oddsValue - betKind.Award);
    }

    public override decimal? GetPlayerOdds(Dictionary<int, decimal> payoutByNumbers)
    {
        foreach (var item in payoutByNumbers) return item.Value;
        return base.GetPlayerOdds(payoutByNumbers);
    }

    public override CompletedTicketResultModel Completed(CompletedTicketModel ticket, List<PrizeMatchResultModel> result)
    {
        var rs = result.Where(f => f.Prize >= _startedPrize && f.Prize <= _endPrize).SelectMany(f => f.Results).Select(f => f.Result).ToList();
        if (rs.Count == 0) return null;
        if (ticket.Children.Count == 0) return null;
        var endOfResults = new List<string>();
        rs.ForEach(f =>
        {
            if (!f.GetEndOfResult(out string val)) return;
            endOfResults.Add(val);
        });
        var groupEndOfResults = endOfResults.GroupBy(f => f).Select(f => new { f.Key, Count = f.Count() }).ToList();
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
            var child = new CompletedChildrenTicketResultModel
            {
                TicketId = item.TicketId
            };
            var playerWinlose = 0m;
            var existedItem = groupEndOfResults.FirstOrDefault(f => f.Key == item.ChoosenNumbers);
            if (existedItem != null)
            {
                child.State = TicketState.Lose;
                child.Times = existedItem.Count;
                playerWinlose = -1 * item.PlayerPayout * existedItem.Count;
            }
            else
            {
                //  Won
                child.State = TicketState.Won;
                playerWinlose = item.Stake * ticket.RewardRate.Value;
            }
            child.PlayerWinLoss = playerWinlose;
            child.AgentWinLoss = -1 * playerWinlose * item.AgentPt;
            child.AgentCommission = (item.PlayerOdds ?? 0m - item.AgentOdds ?? 0m) * item.Stake;

            child.MasterWinLoss = -1 * (item.MasterPt - item.AgentPt) * playerWinlose;
            child.MasterCommission = (item.AgentOdds ?? 0m - item.MasterOdds ?? 0m) * item.Stake;

            child.SupermasterWinLoss = -1 * (item.SupermasterPt - item.MasterPt) * playerWinlose;
            child.SupermasterCommission = (item.MasterOdds ?? 0m - item.SupermasterOdds ?? 0m) * item.Stake;
            child.CompanyWinLoss = -1 * (1 - item.SupermasterPt) * playerWinlose;
            dataResult.Children.Add(child);

            totalPlayerWinLose += playerWinlose;
            totalAgentWinLose += child.AgentWinLoss;
            totalAgentCommission += child.AgentCommission;
            totalMasterWinLose += child.MasterWinLoss;
            totalMasterCommission += child.MasterCommission;
            totalSupermasterWinLose += child.SupermasterWinLoss;
            totalSupermasterCommission += child.SupermasterCommission;
            totalCompanyWinLose += child.CompanyWinLoss;
        }

        dataResult.State = totalPlayerWinLose > 0 ? TicketState.Won : TicketState.Lose;
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
        return null;
    }

    public override RefundRejectTicketResultModel RefundRejectTicket(RefundRejectTicketModel model)
    {
        return null;
    }
}