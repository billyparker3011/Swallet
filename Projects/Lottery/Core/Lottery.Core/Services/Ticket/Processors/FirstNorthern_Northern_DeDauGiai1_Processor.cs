using HnMicro.Core.Helpers;
using HnMicro.Framework.Exceptions;
using Lottery.Core.Enums;
using Lottery.Core.Helpers;
using Lottery.Core.Models.MatchResult;
using Lottery.Core.Models.Ticket;
using Lottery.Core.Models.Ticket.Process;

namespace Lottery.Core.Services.Ticket.Processors;

public class FirstNorthern_Northern_DeDauGiai1_Processor : AbstractBetKindProcessor
{
    private const int _prize = 2;

    public FirstNorthern_Northern_DeDauGiai1_Processor(IServiceProvider serviceProvider) : base(serviceProvider)
    {
    }

    public override int BetKindId { get; set; } = Enums.BetKind.FirstNorthern_Northern_DeDauGiai1.ToInt();

    public override bool EnableStats()
    {
        return true;
    }

    public override int Valid(ProcessTicketModel model, TicketMetadataModel metadata)
    {
        if (!metadata.IsLive) return 0;
        if (metadata.Prize < _prize) return 0;
        else if (metadata.Prize > _prize) return ErrorCodeHelper.ProcessTicket.NotAccepted;
        return !metadata.AllowProcessTicket ? ErrorCodeHelper.ProcessTicket.NotAccepted : 0;
    }

    public override int ValidV2(ProcessTicketV2Model model, List<ProcessValidationTicketDetailV2Model> metadata)
    {
        var metadataItem = metadata.FirstOrDefault(f => f.BetKind != null && f.BetKind.Id == BetKindId) ?? throw new NotFoundException();
        if (metadataItem.Metadata == null) throw new NotFoundException();
        if (!metadataItem.Metadata.IsLive) return 0;
        if (metadataItem.Metadata.Prize < _prize) return 0;
        else if (metadataItem.Metadata.Prize > _prize) return ErrorCodeHelper.ProcessTicket.NotAccepted;
        return !metadataItem.Metadata.AllowProcessTicket ? ErrorCodeHelper.ProcessTicket.NotAccepted : 0;
    }

    public override CompletedTicketResultModel Completed(CompletedTicketModel ticket, List<PrizeMatchResultModel> result)
    {
        var rs = result.FirstOrDefault(f => f.Prize == _prize);
        if (rs == null) return null;
        var latestRs = rs.Results.FirstOrDefault();
        if (latestRs == null) return null;
        if (!latestRs.Result.GetStartOfResult(out string val)) return null;
        var dataResult = new CompletedTicketResultModel
        {
            TicketId = ticket.TicketId
        };
        if (ticket.Children.Count == 0)
        {
            if (ticket.ChoosenNumbers.Equals(val, StringComparison.OrdinalIgnoreCase))
            {
                dataResult.State = TicketState.Won;

                dataResult.PlayerWinLoss = ticket.Stake * ticket.RewardRate.Value - ticket.PlayerPayout;

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
                dataResult.State = TicketState.Lose;

                dataResult.PlayerWinLoss = -1 * ticket.PlayerPayout;

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
        }
        else
        {
            var refundRejectTicketState = CommonHelper.RefundRejectTicketState();

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
                var playerWinlose = 0m;

                var agentWinlose = 0m;
                var agentCommission = 0m;

                var masterWinlose = 0m;
                var masterCommission = 0m;

                var supermasterWinlose = 0m;
                var supermasterCommission = 0m;

                var companyWinlose = 0m;

                if (refundRejectTicketState.Contains(item.State))
                {
                    dataResult.Children.Add(new CompletedChildrenTicketResultModel
                    {
                        TicketId = item.TicketId,
                        State = item.State.ToEnum<TicketState>(),
                        PlayerWinLoss = playerWinlose,
                        AgentWinLoss = agentWinlose,
                        AgentCommission = agentCommission,
                        MasterWinLoss = masterWinlose,
                        MasterCommission = masterCommission,
                        SupermasterWinLoss = supermasterWinlose,
                        SupermasterCommission = supermasterCommission,
                        CompanyWinLoss = companyWinlose
                    });
                }
                else
                {
                    if (item.ChoosenNumbers.Equals(val, StringComparison.OrdinalIgnoreCase))
                    {
                        playerWinlose = item.Stake * ticket.RewardRate.Value - item.PlayerPayout;

                        var agentComm = (item.PlayerOdds ?? 0m) - (item.AgentOdds ?? 0m);
                        if (agentComm < 0m) agentComm = 0m;

                        var masterComm = (item.AgentOdds ?? 0m) - (item.MasterOdds ?? 0m);
                        if (masterComm < 0m) masterComm = 0m;

                        var supermasterComm = (item.MasterOdds ?? 0m) - (item.SupermasterOdds ?? 0m);
                        if (supermasterComm < 0m) supermasterComm = 0m;

                        var commission = GetCommission(item.Stake, agentComm, masterComm, supermasterComm);
                        agentCommission = commission.Item1;
                        masterCommission = commission.Item2;
                        supermasterCommission = commission.Item3;

                        var winlose = GetWinlose(playerWinlose, item.AgentPt, item.MasterPt, item.SupermasterPt);
                        agentWinlose = winlose.Item1;
                        masterWinlose = winlose.Item2;
                        supermasterWinlose = winlose.Item3;
                        companyWinlose = winlose.Item4;

                        dataResult.Children.Add(new CompletedChildrenTicketResultModel
                        {
                            TicketId = item.TicketId,
                            State = TicketState.Won,
                            PlayerWinLoss = playerWinlose,
                            AgentWinLoss = agentWinlose,
                            AgentCommission = agentCommission,
                            MasterWinLoss = masterWinlose,
                            MasterCommission = masterCommission,
                            SupermasterWinLoss = supermasterWinlose,
                            SupermasterCommission = supermasterCommission,
                            CompanyWinLoss = companyWinlose
                        });
                    }
                    else
                    {
                        playerWinlose = -1 * item.PlayerPayout;

                        var agentComm = (item.PlayerOdds ?? 0m) - (item.AgentOdds ?? 0m);
                        if (agentComm < 0m) agentComm = 0m;

                        var masterComm = (item.AgentOdds ?? 0m) - (item.MasterOdds ?? 0m);
                        if (masterComm < 0m) masterComm = 0m;

                        var supermasterComm = (item.MasterOdds ?? 0m) - (item.SupermasterOdds ?? 0m);
                        if (supermasterComm < 0m) supermasterComm = 0m;

                        var commission = GetCommission(item.Stake, agentComm, masterComm, supermasterComm);
                        agentCommission = commission.Item1;
                        masterCommission = commission.Item2;
                        supermasterCommission = commission.Item3;

                        var winlose = GetWinlose(playerWinlose, item.AgentPt, item.MasterPt, item.SupermasterPt);
                        agentWinlose = winlose.Item1;
                        masterWinlose = winlose.Item2;
                        supermasterWinlose = winlose.Item3;
                        companyWinlose = winlose.Item4;

                        dataResult.Children.Add(new CompletedChildrenTicketResultModel
                        {
                            TicketId = item.TicketId,
                            State = TicketState.Lose,
                            PlayerWinLoss = playerWinlose,
                            AgentWinLoss = agentWinlose,
                            AgentCommission = agentCommission,
                            MasterWinLoss = masterWinlose,
                            MasterCommission = masterCommission,
                            SupermasterWinLoss = supermasterWinlose,
                            SupermasterCommission = supermasterCommission,
                            CompanyWinLoss = companyWinlose
                        });
                    }
                }

                totalPlayerWinLose += playerWinlose;

                totalAgentWinLose += agentWinlose;
                totalAgentCommission += agentCommission;

                totalMasterWinLose += masterWinlose;
                totalMasterCommission += masterCommission;

                totalSupermasterWinLose += supermasterWinlose;
                totalSupermasterCommission += supermasterCommission;

                totalCompanyWinLose += companyWinlose;
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
