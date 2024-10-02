using HnMicro.Core.Helpers;
using HnMicro.Framework.Exceptions;
using HnMicro.Modules.InMemory.UnitOfWorks;
using Lottery.Core.Enums;
using Lottery.Core.Helpers;
using Lottery.Core.InMemory.Channel;
using Lottery.Core.InMemory.Setting;
using Lottery.Core.Models.BetKind;
using Lottery.Core.Models.MatchResult;
using Lottery.Core.Models.Setting.ProcessTicket;
using Lottery.Core.Models.Ticket;
using Lottery.Core.Models.Ticket.Process;
using Microsoft.Extensions.DependencyInjection;

namespace Lottery.Core.Services.Ticket.Processors;

public class Central_Mixed_Xien2_Processor : AbstractBetKindProcessor
{
    private const int _numberOfChannels = 2;

    public Central_Mixed_Xien2_Processor(IServiceProvider serviceProvider) : base(serviceProvider)
    {
    }

    public override int BetKindId { get; set; } = Enums.BetKind.Central_Mixed_Xien2.ToInt();

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
            if (listNumbers.Length != 2) return null;

            var firstNumber = listNumbers[0].Trim();
            var timesOfFirstNumber = groupEndOfResults.FirstOrDefault(f => f.Key == firstNumber);

            var secondNumber = listNumbers[1].Trim();
            var timesOfSecondNumber = groupEndOfResults.FirstOrDefault(f => f.Key == secondNumber);

            var isWon = timesOfFirstNumber != null && timesOfSecondNumber != null;
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

            var agentComm = (ticket.PlayerOdds ?? 0m) - (ticket.AgentOdds ?? 0m);
            if (agentComm < 0m) agentComm = 0m;

            var masterComm = (ticket.AgentOdds ?? 0m) - (ticket.MasterOdds ?? 0m);
            if (masterComm < 0m) masterComm = 0m;

            var supermasterComm = (ticket.MasterOdds ?? 0m) - (ticket.SupermasterOdds ?? 0m);
            if (supermasterComm < 0m) supermasterComm = 0m;

            var commission = GetCommission(ticket.Stake, agentComm, masterComm, supermasterComm);
            totalAgentCommission = commission.Item1;
            totalMasterCommission = commission.Item2;
            totalSupermasterCommission = commission.Item3;

            var winlose = GetWinlose(totalPlayerWinLose, ticket.AgentPt, ticket.MasterPt, ticket.SupermasterPt);
            totalAgentWinLose = winlose.Item1;
            totalMasterWinLose = winlose.Item2;
            totalSupermasterWinLose = winlose.Item3;
            totalCompanyWinLose = winlose.Item4;
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
                    playerWinlose = item.Stake * ticket.RewardRate.Value - item.PlayerPayout;
                }
                else
                {
                    //  Lose
                    child.State = TicketState.Lose;
                    playerWinlose = -1 * item.PlayerPayout;
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
        if (listNumbers.Length != 2) return false;

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
            if (timesOfFirstNumber != null && timesOfSecondNumber != null)
            {
                dictResults[channelId] = Math.Max(timesOfFirstNumber.Count, timesOfSecondNumber.Count);
                continue;
            }
            break;
        }
        var isWon = dictResults.Count == _numberOfChannels;
        if (isWon) times = dictResults.Values.Max();
        return isWon;
    }

    public override CompletedTicketResultModel Completed(CompletedTicketModel ticket, Dictionary<int, List<PrizeMatchResultModel>> results)
    {
        var channelIds = ChannelCalculation(ticket.KickoffTime);
        if (channelIds.Count != _numberOfChannels) return null;
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

            var agentComm = (ticket.PlayerOdds ?? 0m) - (ticket.AgentOdds ?? 0m);
            if (agentComm < 0m) agentComm = 0m;

            var masterComm = (ticket.AgentOdds ?? 0m) - (ticket.MasterOdds ?? 0m);
            if (masterComm < 0m) masterComm = 0m;

            var supermasterComm = (ticket.MasterOdds ?? 0m) - (ticket.SupermasterOdds ?? 0m);
            if (supermasterComm < 0m) supermasterComm = 0m;

            var commission = GetCommission(ticket.Stake, agentComm, masterComm, supermasterComm);
            totalAgentCommission = commission.Item1;
            totalMasterCommission = commission.Item2;
            totalSupermasterCommission = commission.Item3;

            var winlose = GetWinlose(totalPlayerWinLose, ticket.AgentPt, ticket.MasterPt, ticket.SupermasterPt);
            totalAgentWinLose = winlose.Item1;
            totalMasterWinLose = winlose.Item2;
            totalSupermasterWinLose = winlose.Item3;
            totalCompanyWinLose = winlose.Item4;
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

    private List<int> ChannelCalculation(DateTime kickOffTime)
    {
        using var scope = ServiceProvider.CreateScope();
        var inMemoryUnitOfWork = scope.ServiceProvider.GetService<IInMemoryUnitOfWork>();
        var channelInMemoryRepository = inMemoryUnitOfWork.GetRepository<IChannelInMemoryRepository>();
        var settingInMemoryRepository = inMemoryUnitOfWork.GetRepository<ISettingInMemoryRepository>();
        var setting = settingInMemoryRepository.FindByKey(nameof(ChannelsForCompletedTicketModel));
        if (setting == null || string.IsNullOrEmpty(setting.ValueSetting)) return new List<int>();

        var data = Newtonsoft.Json.JsonConvert.DeserializeObject<ChannelsForCompletedTicketModel>(setting.ValueSetting);
        var channelIds = new List<int>();
        if (data != null && data.Items.TryGetValue(Region.Central.ToInt(), out List<ChannelsForCompletedTicketDetailModel> vals))
        {
            var configData = vals.FirstOrDefault(f => f.DayOfWeek == kickOffTime.DayOfWeek.ToInt());
            if (configData == null)
            {
                var channelIdsByKickoffTime = channelInMemoryRepository.FindBy(f => f.RegionId == Region.Central.ToInt() && f.DayOfWeeks.Contains(kickOffTime.DayOfWeek.ToInt())).Select(f => f.Id).ToList();
                if (channelIdsByKickoffTime.Count != _numberOfChannels) return new List<int>();
                return channelIdsByKickoffTime;
            }
            foreach (var channelId in configData.ChannelIds)
            {
                if (channelIds.Contains(channelId)) continue;
                channelIds.Add(channelId);
            }
        }
        return channelIds;
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

    public override decimal GetPayoutByNumber(BetKindModel betKind, decimal point, decimal oddsValue, ProcessPayoutMetadataModel metadata = null)
    {
        //  <No.of channels> * <No.of numbers> * 18 *...
        return 2 * 2 * 18 * point * oddsValue;
    }
}
