using HnMicro.Core.Helpers;
using Lottery.Core.Enums;
using Lottery.Core.Helpers;
using Lottery.Core.Models.MatchResult;
using Lottery.Core.Models.Ticket;
using Lottery.Core.Models.Ticket.Process;

namespace Lottery.Core.Services.Ticket.Processors;

public class FirstNorthern_Northern_Xien_2_Processor : AbstractBetKindProcessor
{
    private const int _startedPrize = 2;    //  Exclude Than Tai
    private const int _endPrize = 9;

    public override int BetKindId { get; set; } = Enums.BetKind.FirstNorthern_Northern_Xien2.ToInt();

    public override int Valid(ProcessTicketModel model, TicketMetadataModel metadata)
    {
        return metadata.IsLive ? ErrorCodeHelper.ProcessTicket.NotAccepted : 0;
    }

    public override CompletedTicketResultModel Completed(CompletedTicketModel ticket, List<PrizeMatchResultModel> result)
    {
        var rs = result.Where(f => f.Prize >= _startedPrize && f.Prize <= _endPrize).SelectMany(f => f.Results).Select(f => f.Result).ToList();
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
        if (ticket.Children.Count == 0)
        {
            var choosenNumbers = string.IsNullOrEmpty(ticket.ChoosenNumbers) ? string.Empty : ticket.ChoosenNumbers.Trim();
            if (string.IsNullOrEmpty(choosenNumbers)) return null;

            var listNumbers = choosenNumbers.Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries);
            if (listNumbers.Length != 2) return null;

            var firstNumber = listNumbers[0].Trim();
            var timesOfFirstNumber = groupEndOfResults.FirstOrDefault(f => f.Key == firstNumber);

            var secondNumber = listNumbers[1].Trim();
            var timesOfSecondNumber = groupEndOfResults.FirstOrDefault(f => f.Key == secondNumber);

            var isWon = timesOfFirstNumber != null && timesOfSecondNumber != null;
            if (isWon)
            {
                //  Won
                //var times = 0;
                //var mixedTimes = new Dictionary<string, int>();
                //if (timesOfFirstNumber.Count > 1)
                //{
                //    times += timesOfFirstNumber.Count;
                //    mixedTimes[timesOfFirstNumber.Key] = timesOfFirstNumber.Count;
                //}
                //if (timesOfSecondNumber.Count > 1)
                //{
                //    times += timesOfSecondNumber.Count;
                //    mixedTimes[timesOfSecondNumber.Key] = timesOfSecondNumber.Count;
                //}
                //times = times == 0 ? 1 : times;
                //totalPlayerWinLose = times * ticket.Stake * ticket.RewardRate.Value - ticket.PlayerPayout;
                //ticket.MixedTimes = Newtonsoft.Json.JsonConvert.SerializeObject(mixedTimes);
                totalPlayerWinLose = ticket.Stake * ticket.RewardRate.Value - ticket.PlayerPayout;
            }
            else
            {
                //  Lose
                totalPlayerWinLose = -1 * ticket.PlayerPayout;
            }

            totalAgentWinLose = -1 * totalPlayerWinLose * ticket.AgentPt;
            totalAgentCommission = (ticket.PlayerOdds ?? 0m - ticket.AgentOdds ?? 0m) * ticket.Stake;

            totalMasterWinLose = -1 * (ticket.MasterPt - ticket.AgentPt) * totalPlayerWinLose;
            totalMasterCommission = (ticket.AgentOdds ?? 0m - ticket.MasterOdds ?? 0m) * ticket.Stake;

            totalSupermasterWinLose = -1 * (ticket.SupermasterPt - ticket.MasterPt) * totalPlayerWinLose;
            totalSupermasterCommission = (ticket.MasterOdds ?? 0m - ticket.SupermasterOdds ?? 0m) * ticket.Stake;
            totalCompanyWinLose = -1 * (1 - ticket.SupermasterPt) * totalPlayerWinLose;
        }
        else
        {
            foreach (var item in ticket.Children)
            {
                var choosenNumbers = string.IsNullOrEmpty(item.ChoosenNumbers) ? string.Empty : item.ChoosenNumbers.Trim();
                if (string.IsNullOrEmpty(choosenNumbers)) continue;

                var listNumbers = choosenNumbers.Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries);
                if (listNumbers.Length != 2) continue;

                var firstNumber = listNumbers[0].Trim();
                var timesOfFirstNumber = groupEndOfResults.FirstOrDefault(f => f.Key == firstNumber);

                var secondNumber = listNumbers[1].Trim();
                var timesOfSecondNumber = groupEndOfResults.FirstOrDefault(f => f.Key == secondNumber);

                var child = new CompletedChildrenTicketResultModel
                {
                    TicketId = item.TicketId
                };
                var isWon = timesOfFirstNumber != null && timesOfSecondNumber != null;
                var playerWinlose = 0m;
                if (isWon)
                {
                    //  Won
                    child.State = TicketState.Won;
                    //var times = 0;
                    //var mixedTimes = new Dictionary<string, int>();
                    //if (timesOfFirstNumber.Count > 1)
                    //{
                    //    times += timesOfFirstNumber.Count;
                    //    mixedTimes[timesOfFirstNumber.Key] = timesOfFirstNumber.Count;
                    //}
                    //if (timesOfSecondNumber.Count > 1)
                    //{
                    //    times += timesOfSecondNumber.Count;
                    //    mixedTimes[timesOfSecondNumber.Key] = timesOfSecondNumber.Count;
                    //}
                    //times = times == 0 ? 1 : times;
                    //playerWinlose = times * item.Stake * ticket.RewardRate.Value - item.PlayerPayout;
                    //child.MixedTimes = Newtonsoft.Json.JsonConvert.SerializeObject(mixedTimes);
                    playerWinlose = item.Stake * ticket.RewardRate.Value - item.PlayerPayout;
                }
                else
                {
                    //  Lose
                    child.State = TicketState.Lose;
                    playerWinlose = -1 * item.PlayerPayout;
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
        return dataResult;
    }
}
