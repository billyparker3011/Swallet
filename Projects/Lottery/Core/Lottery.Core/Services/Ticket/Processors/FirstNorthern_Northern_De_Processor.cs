using HnMicro.Core.Helpers;
using Lottery.Core.Enums;
using Lottery.Core.Helpers;
using Lottery.Core.Models.MatchResult;
using Lottery.Core.Models.Ticket;
using Lottery.Core.Models.Ticket.Process;

namespace Lottery.Core.Services.Ticket.Processors;

public class FirstNorthern_Northern_De_Processor : AbstractBetKindProcessor
{
    private const int _prize = 9;
    private const int _acceptedPrize = 6;

    public override int BetKindId { get; set; } = Enums.BetKind.FirstNorthern_Northern_De.ToInt();

    public override bool EnableStats()
    {
        return true;
    }

    public override int Valid(ProcessTicketModel model, TicketMetadataModel metadata)
    {
        if (!metadata.IsLive) return 0;
        return metadata.Prize.HasValue && metadata.Prize.Value > _acceptedPrize ? ErrorCodeHelper.ProcessTicket.NotAccepted : 0;
    }

    public override CompletedTicketResultModel Completed(CompletedTicketModel ticket, List<PrizeMatchResultModel> result)
    {
        var rs = result.FirstOrDefault(f => f.Prize == _prize);
        if (rs == null) return null;
        var latestRs = rs.Results.FirstOrDefault();
        if (latestRs == null) return null;
        if (!latestRs.Result.GetEndOfResult(out string val)) return null;
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

                dataResult.AgentWinLoss = -1 * dataResult.PlayerWinLoss * ticket.AgentPt;
                dataResult.AgentCommission = (ticket.PlayerOdds ?? 0m - ticket.AgentOdds ?? 0m) * ticket.Stake;

                dataResult.MasterWinLoss = -1 * (ticket.MasterPt - ticket.AgentPt) * dataResult.PlayerWinLoss;
                dataResult.MasterCommission = (ticket.AgentOdds ?? 0m - ticket.MasterOdds ?? 0m) * ticket.Stake;

                dataResult.SupermasterWinLoss = -1 * (ticket.SupermasterPt - ticket.MasterPt) * dataResult.PlayerWinLoss;
                dataResult.SupermasterCommission = (ticket.MasterOdds ?? 0m - ticket.SupermasterOdds ?? 0m) * ticket.Stake;

                dataResult.CompanyWinLoss = -1 * (1 - ticket.SupermasterPt) * dataResult.PlayerWinLoss;
            }
            else
            {
                dataResult.State = TicketState.Lose;

                dataResult.PlayerWinLoss = -1 * ticket.PlayerPayout;

                dataResult.AgentWinLoss = -1 * dataResult.PlayerWinLoss * ticket.AgentPt;
                dataResult.AgentCommission = (ticket.PlayerOdds ?? 0m - ticket.AgentOdds ?? 0m) * ticket.Stake;

                dataResult.MasterWinLoss = -1 * (ticket.MasterPt - ticket.AgentPt) * dataResult.PlayerWinLoss;
                dataResult.MasterCommission = (ticket.AgentOdds ?? 0m - ticket.MasterOdds ?? 0m) * ticket.Stake;

                dataResult.SupermasterWinLoss = -1 * (ticket.SupermasterPt - ticket.MasterPt) * dataResult.PlayerWinLoss;
                dataResult.SupermasterCommission = (ticket.MasterOdds ?? 0m - ticket.SupermasterOdds ?? 0m) * ticket.Stake;

                dataResult.CompanyWinLoss = -1 * (1 - ticket.SupermasterPt) * dataResult.PlayerWinLoss;
            }
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
                var playerWinlose = 0m;

                var agentWinlose = 0m;
                var agentCommission = 0m;

                var masterWinlose = 0m;
                var masterCommission = 0m;

                var supermasterWinlose = 0m;
                var supermasterCommission = 0m;

                var companyWinlose = 0m;
                if (item.ChoosenNumbers.Equals(val, StringComparison.OrdinalIgnoreCase))
                {
                    playerWinlose = item.Stake * ticket.RewardRate.Value - item.PlayerPayout;

                    agentWinlose = -1 * playerWinlose * item.AgentPt;
                    agentCommission = (item.PlayerOdds ?? 0m - item.AgentOdds ?? 0m) * item.Stake;

                    masterWinlose = -1 * (item.MasterPt - item.AgentPt) * playerWinlose;
                    masterCommission = (item.AgentOdds ?? 0m - item.MasterOdds ?? 0m) * item.Stake;

                    supermasterWinlose = -1 * (item.SupermasterPt - item.MasterPt) * playerWinlose;
                    supermasterCommission = (item.MasterOdds ?? 0m - item.SupermasterOdds ?? 0m) * item.Stake;

                    companyWinlose = -1 * (1 - item.SupermasterPt) * playerWinlose;

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

                    agentWinlose = -1 * playerWinlose * item.AgentPt;
                    agentCommission = (item.PlayerOdds ?? 0m - item.AgentOdds ?? 0m) * item.Stake;

                    masterWinlose = -1 * (item.MasterPt - item.AgentPt) * playerWinlose;
                    masterCommission = (item.AgentOdds ?? 0m - item.MasterOdds ?? 0m) * item.Stake;

                    supermasterWinlose = -1 * (item.SupermasterPt - item.MasterPt) * playerWinlose;
                    supermasterCommission = (item.MasterOdds ?? 0m - item.SupermasterOdds ?? 0m) * item.Stake;

                    companyWinlose = -1 * (1 - item.SupermasterPt) * playerWinlose;

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
