using HnMicro.Core.Helpers;
using HnMicro.Framework.Exceptions;
using Lottery.Core.Enums;
using Lottery.Core.Helpers;
using Lottery.Core.InMemory.Setting;
using Lottery.Core.Models.MatchResult;
using Lottery.Core.Models.Setting.ProcessTicket;
using Lottery.Core.Models.Ticket;
using Lottery.Core.Models.Ticket.Process;
using Microsoft.Extensions.DependencyInjection;

namespace Lottery.Core.Services.Ticket.Processors;

public class Central_Mixed_Xien4_Processor : AbstractBetKindProcessor
{
    public Central_Mixed_Xien4_Processor(IServiceProvider serviceProvider) : base(serviceProvider)
    {
    }

    public override int BetKindId { get; set; } = Enums.BetKind.Central_Mixed_Xien4.ToInt();

    public override int Valid(ProcessTicketModel model, TicketMetadataModel metadata)
    {
        return metadata.IsLive ? ErrorCodeHelper.ProcessTicket.NotAccepted : 0;
    }

    public override int ValidV2(ProcessTicketV2Model model, List<ProcessValidationTicketDetailV2Model> metadata)
    {
        var metadataItem = metadata.FirstOrDefault(f => f.BetKind != null && f.BetKind.Id == BetKindId) ?? throw new NotFoundException();
        if (metadataItem.Metadata == null) throw new NotFoundException();
        return metadataItem.Metadata.IsLive ? ErrorCodeHelper.ProcessTicket.NotAccepted : 0;
    }

    public override CompletedTicketResultModel Completed(CompletedTicketModel ticket, List<PrizeMatchResultModel> result)
    {
        var rs = result.SelectMany(f => f.Results).Select(f => f.Result).ToList();
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
            if (listNumbers.Length != 4) return null;

            var firstNumber = listNumbers[0].Trim();
            var timesOfFirstNumber = groupEndOfResults.FirstOrDefault(f => f.Key == firstNumber);

            var secondNumber = listNumbers[1].Trim();
            var timesOfSecondNumber = groupEndOfResults.FirstOrDefault(f => f.Key == secondNumber);

            var thirdNumber = listNumbers[2].Trim();
            var timesOfThirdNumber = groupEndOfResults.FirstOrDefault(f => f.Key == thirdNumber);

            var fourthNumber = listNumbers[3].Trim();
            var timesOfFourthNumber = groupEndOfResults.FirstOrDefault(f => f.Key == fourthNumber);

            var isWon = timesOfFirstNumber != null && timesOfSecondNumber != null && timesOfThirdNumber != null && timesOfFourthNumber != null;
            if (isWon)
            {
                //  Won
                totalPlayerWinLose = ticket.Stake * ticket.RewardRate.Value - ticket.PlayerPayout;
            }
            else
            {
                //  Lose
                totalPlayerWinLose = -1 * ticket.PlayerPayout;
            }

            totalAgentWinLose = -1 * totalPlayerWinLose * ticket.AgentPt;
            var agentComm = (ticket.PlayerOdds ?? 0m) - (ticket.AgentOdds ?? 0m);
            if (agentComm < 0m) agentComm = 0m;
            totalAgentCommission = agentComm * ticket.Stake;

            totalMasterWinLose = -1 * (ticket.MasterPt - ticket.AgentPt) * totalPlayerWinLose;
            var masterComm = (ticket.AgentOdds ?? 0m) - (ticket.MasterOdds ?? 0m);
            if (masterComm < 0m) masterComm = 0m;
            totalMasterCommission = masterComm * ticket.Stake;

            totalSupermasterWinLose = -1 * (ticket.SupermasterPt - ticket.MasterPt) * totalPlayerWinLose;
            var supermasterComm = (ticket.MasterOdds ?? 0m) - (ticket.SupermasterOdds ?? 0m);
            if (supermasterComm < 0m) supermasterComm = 0m;
            totalSupermasterCommission = supermasterComm * ticket.Stake;

            totalCompanyWinLose = -1 * (1 - ticket.SupermasterPt) * totalPlayerWinLose;
        }
        else
        {
            foreach (var item in ticket.Children)
            {
                var choosenNumbers = string.IsNullOrEmpty(item.ChoosenNumbers) ? string.Empty : item.ChoosenNumbers.Trim();
                if (string.IsNullOrEmpty(choosenNumbers)) continue;

                var listNumbers = choosenNumbers.Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries);
                if (listNumbers.Length != 4) continue;

                var firstNumber = listNumbers[0].Trim();
                var timesOfFirstNumber = groupEndOfResults.FirstOrDefault(f => f.Key == firstNumber);

                var secondNumber = listNumbers[1].Trim();
                var timesOfSecondNumber = groupEndOfResults.FirstOrDefault(f => f.Key == secondNumber);

                var thirdNumber = listNumbers[2].Trim();
                var timesOfThirdNumber = groupEndOfResults.FirstOrDefault(f => f.Key == thirdNumber);

                var fourthNumber = listNumbers[3].Trim();
                var timesOfFourthNumber = groupEndOfResults.FirstOrDefault(f => f.Key == fourthNumber);

                var child = new CompletedChildrenTicketResultModel
                {
                    TicketId = item.TicketId
                };
                var isWon = timesOfFirstNumber != null && timesOfSecondNumber != null && timesOfThirdNumber != null && timesOfFourthNumber != null;
                var playerWinlose = 0m;
                if (isWon)
                {
                    //  Won
                    child.State = TicketState.Won;
                    playerWinlose = item.Stake * ticket.RewardRate.Value - item.PlayerPayout;
                }
                else
                {
                    //  Lose
                    child.State = TicketState.Lose;
                    playerWinlose = -1 * item.PlayerPayout;
                }
                child.PlayerWinLoss = playerWinlose;
                child.AgentWinLoss = -1 * playerWinlose * item.AgentPt;
                var agentComm = (item.PlayerOdds ?? 0m) - (item.AgentOdds ?? 0m);
                if (agentComm < 0m) agentComm = 0m;
                child.AgentCommission = agentComm * item.Stake;

                child.MasterWinLoss = -1 * (item.MasterPt - item.AgentPt) * playerWinlose;
                var masterComm = (item.AgentOdds ?? 0m) - (item.MasterOdds ?? 0m);
                if (masterComm < 0m) masterComm = 0m;
                child.MasterCommission = masterComm * item.Stake;

                child.SupermasterWinLoss = -1 * (item.SupermasterPt - item.MasterPt) * playerWinlose;
                var supermasterComm = (item.MasterOdds ?? 0m) - (item.SupermasterOdds ?? 0m);
                if (supermasterComm < 0m) supermasterComm = 0m;
                child.SupermasterCommission = supermasterComm * item.Stake;

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

    private bool CheckOnPairChannels(string choosenNumbers, Dictionary<int, List<PrizeMatchResultModel>> results, List<int> calculatedChannelIds, out int times)
    {
        times = 0;

        var listNumbers = choosenNumbers.Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries);
        if (listNumbers.Length != 4) return false;

        var dictResults = new Dictionary<int, int>();
        foreach (var channelId in calculatedChannelIds)
        {
            if (!results.TryGetValue(channelId, out List<PrizeMatchResultModel> resultByChannel)) break;

            var rs = resultByChannel.SelectMany(f => f.Results).Select(f => f.Result).ToList();
            var endOfResults = new List<string>();
            rs.ForEach(f =>
            {
                if (!f.GetEndOfResult(out string val)) return;
                endOfResults.Add(val);
            });
            var groupEndOfResults = endOfResults.GroupBy(f => f).Select(f => new { f.Key, Count = f.Count() }).ToList();

            var firstNumber = listNumbers[0].Trim();
            var timesOfFirstNumber = groupEndOfResults.FirstOrDefault(f => f.Key == firstNumber);

            var secondNumber = listNumbers[1].Trim();
            var timesOfSecondNumber = groupEndOfResults.FirstOrDefault(f => f.Key == secondNumber);

            var thirdNumber = listNumbers[2].Trim();
            var timesOfThirdNumber = groupEndOfResults.FirstOrDefault(f => f.Key == thirdNumber);

            var fourthNumber = listNumbers[3].Trim();
            var timesOfFourthNumber = groupEndOfResults.FirstOrDefault(f => f.Key == fourthNumber);

            if (timesOfFirstNumber != null && timesOfSecondNumber != null && timesOfThirdNumber != null && timesOfFourthNumber != null)
            {
                dictResults[channelId] = Math.Max(Math.Max(Math.Max(timesOfFirstNumber.Count, timesOfSecondNumber.Count), timesOfThirdNumber.Count), timesOfFourthNumber.Count);
                continue;
            }
            break;
        }
        var isWon = dictResults.Count == 2;
        if (isWon) times = dictResults.Values.Max();
        return isWon;
    }

    private List<int> ChannelCalculation(DateTime kickOffTime)
    {
        using var scope = ServiceProvider.CreateScope();
        var settingInMemoryRepository = scope.ServiceProvider.GetService<ISettingInMemoryRepository>();
        var setting = settingInMemoryRepository.FindByKey(nameof(ChannelsForCompletedTicketModel));
        if (setting == null || string.IsNullOrEmpty(setting.ValueSetting)) return new List<int>();

        var data = Newtonsoft.Json.JsonConvert.DeserializeObject<ChannelsForCompletedTicketModel>(setting.ValueSetting);
        var channelIds = new List<int>();
        if (data != null && data.Items.TryGetValue(Region.Central.ToInt(), out List<ChannelsForCompletedTicketDetailModel> vals))
        {
            var configData = vals.FirstOrDefault(f => f.DayOfWeek == kickOffTime.DayOfWeek.ToInt());
            if (configData == null) return new List<int>();
            foreach (var channelId in configData.ChannelIds)
            {
                if (channelIds.Contains(channelId)) continue;
                channelIds.Add(channelId);
            }
        }
        return channelIds;
    }

    public override CompletedTicketResultModel Completed(CompletedTicketModel ticket, Dictionary<int, List<PrizeMatchResultModel>> results)
    {
        var channelIds = ChannelCalculation(ticket.KickoffTime);
        if (channelIds.Count != 2) return null;
        foreach (var channelId in channelIds)
        {
            if (results.ContainsKey(channelId)) continue;
            return null;
        }

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

            var isWon = CheckOnPairChannels(choosenNumbers, results, channelIds, out int times);
            if (isWon)
            {
                //  Won
                totalPlayerWinLose = times * ticket.Stake * ticket.RewardRate.Value - ticket.PlayerPayout;
            }
            else
            {
                //  Lose
                totalPlayerWinLose = -1 * ticket.PlayerPayout;
            }

            totalAgentWinLose = -1 * totalPlayerWinLose * ticket.AgentPt;
            var agentComm = (ticket.PlayerOdds ?? 0m) - (ticket.AgentOdds ?? 0m);
            if (agentComm < 0m) agentComm = 0m;
            totalAgentCommission = agentComm * ticket.Stake;

            totalMasterWinLose = -1 * (ticket.MasterPt - ticket.AgentPt) * totalPlayerWinLose;
            var masterComm = (ticket.AgentOdds ?? 0m) - (ticket.MasterOdds ?? 0m);
            if (masterComm < 0m) masterComm = 0m;
            totalMasterCommission = masterComm * ticket.Stake;

            totalSupermasterWinLose = -1 * (ticket.SupermasterPt - ticket.MasterPt) * totalPlayerWinLose;
            var supermasterComm = (ticket.MasterOdds ?? 0m) - (ticket.SupermasterOdds ?? 0m);
            if (supermasterComm < 0m) supermasterComm = 0m;
            totalSupermasterCommission = supermasterComm * ticket.Stake;

            totalCompanyWinLose = -1 * (1 - ticket.SupermasterPt) * totalPlayerWinLose;
        }
        else
        {
            foreach (var item in ticket.Children)
            {
                var choosenNumbers = string.IsNullOrEmpty(item.ChoosenNumbers) ? string.Empty : item.ChoosenNumbers.Trim();
                if (string.IsNullOrEmpty(choosenNumbers)) continue;

                var child = new CompletedChildrenTicketResultModel
                {
                    TicketId = item.TicketId
                };
                var isWon = CheckOnPairChannels(choosenNumbers, results, channelIds, out int times);
                var playerWinlose = 0m;
                if (isWon)
                {
                    //  Won
                    child.State = TicketState.Won;
                    playerWinlose = times * item.Stake * ticket.RewardRate.Value - item.PlayerPayout;
                }
                else
                {
                    //  Lose
                    child.State = TicketState.Lose;
                    playerWinlose = -1 * item.PlayerPayout;
                }
                child.PlayerWinLoss = playerWinlose;

                child.AgentWinLoss = -1 * playerWinlose * item.AgentPt;
                var agentComm = (item.PlayerOdds ?? 0m) - (item.AgentOdds ?? 0m);
                if (agentComm < 0m) agentComm = 0m;
                child.AgentCommission = agentComm * item.Stake;

                child.MasterWinLoss = -1 * (item.MasterPt - item.AgentPt) * playerWinlose;
                var masterComm = (item.AgentOdds ?? 0m) - (item.MasterOdds ?? 0m);
                if (masterComm < 0m) masterComm = 0m;
                child.MasterCommission = masterComm * item.Stake;

                child.SupermasterWinLoss = -1 * (item.SupermasterPt - item.MasterPt) * playerWinlose;
                var supermasterComm = (item.MasterOdds ?? 0m) - (item.SupermasterOdds ?? 0m);
                if (supermasterComm < 0m) supermasterComm = 0m;
                child.SupermasterCommission = supermasterComm * item.Stake;

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
        var result = new RefundRejectTicketByNumbersResultModel
        {
            Allow = true,
            Ticket = model.Ticket,
            Children = model.Children
        };
        var enableStats = EnableStats();
        if (model.Children.Count == 0)
        {
            var choosenNumbers = SplitChooseNumbers(model.Ticket.ChoosenNumbers);
            if (choosenNumbers.Any(model.RefundRejectNumbers.Contains))
            {
                result.Ticket.State = model.Ticket.IsLive.GetRefundRejectStateByIsLive();
                result.DifferentPlayerPayout += model.Ticket.PlayerPayout;
                //result.OutsByNumbers[int.Parse(model.Ticket.ChoosenNumbers)] = model.Ticket.PlayerPayout;
                //result.PointsByNumbers[int.Parse(model.Ticket.ChoosenNumbers)] = model.Ticket.Stake;
                //if (enableStats)
                //{
                //    result.OutsByBetKind[int.Parse(model.Ticket.ChoosenNumbers)] = model.Ticket.PlayerPayout;
                //    result.PointsByBetKind[int.Parse(model.Ticket.ChoosenNumbers)] = model.Ticket.Stake;
                //}
            }
        }
        else
        {
            var refundRejectTicketState = CommonHelper.RefundRejectTicketState();
            foreach (var item in result.Children)
            {
                var choosenNumbers = SplitChooseNumbers(item.ChoosenNumbers);
                if (choosenNumbers.Any(model.RefundRejectNumbers.Contains) && !refundRejectTicketState.Contains(item.State.ToInt()))
                {
                    item.State = item.IsLive.GetRefundRejectStateByIsLive();
                    result.DifferentPlayerPayout += item.PlayerPayout;
                    //result.OutsByNumbers[int.Parse(item.ChoosenNumbers)] = item.PlayerPayout;
                    //result.PointsByNumbers[int.Parse(item.ChoosenNumbers)] = item.Stake;
                    //if (enableStats)
                    //{
                    //    result.OutsByBetKind[int.Parse(item.ChoosenNumbers)] = item.PlayerPayout;
                    //    result.PointsByBetKind[int.Parse(item.ChoosenNumbers)] = item.Stake;
                    //}
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

    public override RefundRejectTicketResultModel RefundRejectTicket(RefundRejectTicketModel model)
    {
        var result = new RefundRejectTicketResultModel();
        //var enableStats = EnableStats();
        if (model.Children.Count == 0)
        {
            result.DifferentPlayerPayout += model.Ticket.PlayerPayout;
            //result.OutsByNumbers[int.Parse(model.Ticket.ChoosenNumbers)] = model.Ticket.PlayerPayout;
            //result.PointsByNumbers[int.Parse(model.Ticket.ChoosenNumbers)] = model.Ticket.Stake;
            //if (enableStats)
            //{
            //    result.OutsByBetKind[int.Parse(model.Ticket.ChoosenNumbers)] = model.Ticket.PlayerPayout;
            //    result.PointsByBetKind[int.Parse(model.Ticket.ChoosenNumbers)] = model.Ticket.Stake;
            //}
        }
        else
        {
            var refundRejectTicketState = CommonHelper.RefundRejectTicketState();
            foreach (var item in model.Children)
            {
                if (!refundRejectTicketState.Contains(item.State.ToInt()))
                {
                    result.DifferentPlayerPayout += item.PlayerPayout;
                    //result.OutsByNumbers[int.Parse(item.ChoosenNumbers)] = item.PlayerPayout;
                    //result.PointsByNumbers[int.Parse(item.ChoosenNumbers)] = item.Stake;
                    //if (enableStats)
                    //{
                    //    result.OutsByBetKind[int.Parse(item.ChoosenNumbers)] = item.PlayerPayout;
                    //    result.PointsByBetKind[int.Parse(item.ChoosenNumbers)] = item.Stake;
                    //}
                }
            }
        }
        return result;
    }
}