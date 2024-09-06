using HnMicro.Core.Helpers;
using HnMicro.Framework.Exceptions;
using Lottery.Core.Enums;
using Lottery.Core.Helpers;
using Lottery.Core.Models.BetKind;
using Lottery.Core.Models.MatchResult;
using Lottery.Core.Models.Ticket;
using Lottery.Core.Models.Ticket.Process;

namespace Lottery.Core.Services.Ticket.Processors;

public class SecondNorthern_Northern_3D23Lo_Processor : AbstractBetKindProcessor
{
    private const int _resultPrize = 9;
    private const int _resultFromPrize = 2;
    private const int _resultToPrize = 7;

    public SecondNorthern_Northern_3D23Lo_Processor(IServiceProvider serviceProvider) : base(serviceProvider)
    {
    }

    public override int BetKindId { get; set; } = Enums.BetKind.SecondNorthern_Northern_3D23Lo.ToInt();

    public override int Valid(ProcessTicketModel model, TicketMetadataModel metadata)
    {
        if (metadata.IsLive) return ErrorCodeHelper.ProcessTicket.NotAccepted;
        return model.Numbers.Count > NoOfSelectedNumbersExceed ? ErrorCodeHelper.ProcessTicket.NoOfSelectedNumbersExceed512 : 0;
    }

    public override int ValidV2(ProcessTicketV2Model model, List<ProcessValidationTicketDetailV2Model> metadata)
    {
        var metadataItem = metadata.FirstOrDefault(f => f.BetKind != null && f.BetKind.Id == BetKindId) ?? throw new NotFoundException();
        if (metadataItem.Metadata == null) throw new NotFoundException();
        if (metadataItem.Metadata.IsLive) return ErrorCodeHelper.ProcessTicket.NotAccepted;
        var betKindDetail = model.Details.FirstOrDefault(f => f.BetKindId == BetKindId) ?? throw new NotFoundException();
        return betKindDetail.Numbers.Count > NoOfSelectedNumbersExceed ? ErrorCodeHelper.ProcessTicket.NoOfSelectedNumbersExceed512 : 0;
    }

    public override decimal GetPayoutByNumber(BetKindModel betKind, decimal point, decimal oddsValue, ProcessPayoutMetadataModel metadata = null)
    {
        return 23 * oddsValue * point;
    }

    public override CompletedTicketResultModel Completed(CompletedTicketModel ticket, List<PrizeMatchResultModel> result)
    {
        var rs = result.Where(f => f.Prize == _resultPrize || (f.Prize >= _resultFromPrize && f.Prize <= _resultToPrize)).SelectMany(f => f.Results).Select(f => f.Result).ToList();
        var endOfResults = new List<string>();
        rs.ForEach(f =>
        {
            if (!f.GetEndOfResult(out string val, 3)) return;
            endOfResults.Add(val);
        });
        var groupEndOfResults = endOfResults.GroupBy(f => f).Select(f => new { f.Key, Count = f.Count() }).ToList();
        var dataResult = new CompletedTicketResultModel
        {
            TicketId = ticket.TicketId
        };
        if (ticket.Children.Count == 0)
        {
            var findChooseNumbers = groupEndOfResults.FirstOrDefault(f => f.Key == ticket.ChoosenNumbers);
            if (findChooseNumbers == null)
            {
                dataResult.State = TicketState.Lose;
                dataResult.PlayerWinLoss = -1 * ticket.PlayerPayout;
            }
            else
            {
                dataResult.State = TicketState.Won;
                dataResult.PlayerWinLoss = findChooseNumbers.Count * ticket.Stake * ticket.RewardRate.Value - ticket.PlayerPayout;
                dataResult.Times = findChooseNumbers.Count;
            }

            var agentComm = (ticket.PlayerOdds ?? 0m) - (ticket.AgentOdds ?? 0m);
            if (agentComm < 0m) agentComm = 0m;

            var masterComm = (ticket.AgentOdds ?? 0m) - (ticket.MasterOdds ?? 0m);
            if (masterComm < 0m) masterComm = 0m;

            var supermasterComm = (ticket.MasterOdds ?? 0m) - (ticket.SupermasterOdds ?? 0m);
            if (supermasterComm < 0m) supermasterComm = 0m;

            var commission = GetCommission(ticket.Stake, agentComm, masterComm, supermasterComm);
            dataResult.AgentCommission = commission.Item1;
            dataResult.MasterCommission = commission.Item2;
            dataResult.SupermasterCommission = commission.Item3;

            var winlose = GetWinlose(dataResult.PlayerWinLoss, ticket.AgentPt, ticket.MasterPt, ticket.SupermasterPt);
            dataResult.AgentWinLoss = winlose.Item1;
            dataResult.MasterWinLoss = winlose.Item2;
            dataResult.SupermasterWinLoss = winlose.Item3;
            dataResult.CompanyWinLoss = winlose.Item4;
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
                var findChooseNumbers = groupEndOfResults.FirstOrDefault(f => f.Key == item.ChoosenNumbers);
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
                    child.Times = findChooseNumbers.Count;
                    playerWinlose = findChooseNumbers.Count * item.Stake * ticket.RewardRate.Value - item.PlayerPayout;
                }
                child.PlayerWinLoss = playerWinlose;

                var agentComm = (item.PlayerOdds ?? 0m) - (item.AgentOdds ?? 0m);
                if (agentComm < 0m) agentComm = 0m;

                var masterComm = (item.AgentOdds ?? 0m) - (item.MasterOdds ?? 0m);
                if (masterComm < 0m) masterComm = 0m;

                var supermasterComm = (item.MasterOdds ?? 0m) - (item.SupermasterOdds ?? 0m);
                if (supermasterComm < 0m) supermasterComm = 0m;

                var commission = GetCommission(item.Stake, agentComm, masterComm, supermasterComm);
                child.AgentCommission = commission.Item1;
                child.MasterCommission = commission.Item2;
                child.SupermasterCommission = commission.Item3;

                var winlose = GetWinlose(playerWinlose, item.AgentPt, item.MasterPt, item.SupermasterPt);
                child.AgentWinLoss = winlose.Item1;
                child.MasterWinLoss = winlose.Item2;
                child.SupermasterWinLoss = winlose.Item3;
                child.CompanyWinLoss = winlose.Item4;

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
        }
        return dataResult;
    }
}

