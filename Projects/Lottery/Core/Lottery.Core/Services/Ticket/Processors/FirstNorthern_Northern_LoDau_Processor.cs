using HnMicro.Core.Helpers;
using Lottery.Core.Enums;
using Lottery.Core.Helpers;
using Lottery.Core.Models.MatchResult;
using Lottery.Core.Models.Ticket;
using Lottery.Core.Models.Ticket.Process;

namespace Lottery.Core.Services.Ticket.Processors;

public class FirstNorthern_Northern_LoDau_Processor : AbstractBetKindProcessor
{
    private const int _startedPrize = 2;
    private const int _endPrize = 9;

    public override int BetKindId { get; set; } = Enums.BetKind.FirstNorthern_Northern_LoDau.ToInt();

    public override int Valid(ProcessTicketModel model, TicketMetadataModel metadata)
    {
        return metadata.IsLive ? ErrorCodeHelper.ProcessTicket.NotAccepted : 0;
    }

    public override CompletedTicketResultModel Completed(CompletedTicketModel ticket, List<PrizeMatchResultModel> result)
    {
        var rs = result.Where(f => f.Prize >= _startedPrize && f.Prize <= _endPrize).SelectMany(f => f.Results).Select(f => f.Result).ToList();
        var startOfResults = new List<string>();
        rs.ForEach(f =>
        {
            if (!f.GetStartOfResult(out string val)) return;
            startOfResults.Add(val);
        });
        var groupStartOfResults = startOfResults.GroupBy(f => f).Select(f => new { f.Key, Count = f.Count() }).ToList();
        var dataResult = new CompletedTicketResultModel
        {
            TicketId = ticket.TicketId
        };
        if (ticket.Children.Count == 0)
        {
            var findChooseNumbers = groupStartOfResults.FirstOrDefault(f => f.Key == ticket.ChoosenNumbers);
            if (findChooseNumbers == null)
            {
                dataResult.State = TicketState.Lose;
                dataResult.PlayerWinLose = -1 * ticket.PlayerPayout;
            }
            else
            {
                dataResult.State = TicketState.Won;
                dataResult.PlayerWinLose = findChooseNumbers.Count * ticket.Stake * ticket.RewardRate.Value - ticket.PlayerPayout;
            }

            dataResult.AgentWinLose = -1 * dataResult.PlayerWinLose * ticket.AgentPt;
            dataResult.AgentCommission = (ticket.PlayerOdds ?? 0m - ticket.AgentOdds ?? 0m) * ticket.Stake;

            dataResult.MasterWinLose = -1 * (ticket.MasterPt - ticket.AgentPt) * dataResult.PlayerWinLose;
            dataResult.MasterCommission = (ticket.AgentOdds ?? 0m - ticket.MasterOdds ?? 0m) * ticket.Stake;

            dataResult.SupermasterWinLose = -1 * (ticket.SupermasterPt - ticket.MasterPt) * dataResult.PlayerWinLose;
            dataResult.SupermasterCommission = (ticket.MasterOdds ?? 0m - ticket.SupermasterOdds ?? 0m) * ticket.Stake;

            dataResult.CompanyWinLose = -1 * (1 - ticket.SupermasterPt) * dataResult.PlayerWinLose;
        }
        else
        {
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
                var findChooseNumbers = groupStartOfResults.FirstOrDefault(f => f.Key == item.ChoosenNumbers);
                if (findChooseNumbers == null)
                {
                    //  Lose
                    child.State = TicketState.Lose;
                    playerWinlose = -1 * item.PlayerPayout;
                }
                else
                {
                    //  Won
                    child.State = TicketState.Won;
                    playerWinlose = findChooseNumbers.Count * item.Stake * ticket.RewardRate.Value - item.PlayerPayout;
                }
                child.PlayerWinLose = playerWinlose;
                child.AgentWinLose = -1 * playerWinlose * item.AgentPt;
                child.AgentCommission = (item.PlayerOdds ?? 0m - item.AgentOdds ?? 0m) * item.Stake;

                child.MasterWinLose = -1 * (item.MasterPt - item.AgentPt) * playerWinlose;
                child.MasterCommission = (item.AgentOdds ?? 0m - item.MasterOdds ?? 0m) * item.Stake;

                child.SupermasterWinLose = -1 * (item.SupermasterPt - item.MasterPt) * playerWinlose;
                child.SupermasterCommission = (item.MasterOdds ?? 0m - item.SupermasterOdds ?? 0m) * item.Stake;
                child.CompanyWinLose = -1 * (1 - item.SupermasterPt) * playerWinlose;
                dataResult.Children.Add(child);

                totalPlayerWinLose += playerWinlose;
                totalAgentWinLose += child.AgentWinLose;
                totalAgentCommission += child.AgentCommission;
                totalMasterWinLose += child.MasterWinLose;
                totalMasterCommission += child.MasterCommission;
                totalSupermasterWinLose += child.SupermasterWinLose;
                totalSupermasterCommission += child.SupermasterCommission;
                totalCompanyWinLose += child.CompanyWinLose;
            }

            dataResult.State = totalPlayerWinLose > 0 ? TicketState.Won : TicketState.Lose;
            dataResult.PlayerWinLose = totalPlayerWinLose;
            dataResult.AgentWinLose = totalAgentWinLose;
            dataResult.AgentCommission = totalAgentCommission;
            dataResult.MasterWinLose = totalMasterWinLose;
            dataResult.MasterCommission = totalMasterCommission;
            dataResult.SupermasterWinLose = totalSupermasterWinLose;
            dataResult.SupermasterCommission = totalSupermasterCommission;
            dataResult.CompanyWinLose = totalCompanyWinLose;
        }
        return dataResult;
    }
}
