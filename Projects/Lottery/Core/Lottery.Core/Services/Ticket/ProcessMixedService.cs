using HnMicro.Core.Helpers;
using HnMicro.Framework.Enums;
using HnMicro.Framework.Exceptions;
using HnMicro.Framework.Services;
using HnMicro.Modules.InMemory.UnitOfWorks;
using Lottery.Core.Contexts;
using Lottery.Core.Enums;
using Lottery.Core.Helpers;
using Lottery.Core.InMemory.BetKind;
using Lottery.Core.Models.BetKind;
using Lottery.Core.Models.PositionTakings;
using Lottery.Core.Models.Setting;
using Lottery.Core.Models.Ticket.Process;
using Lottery.Core.Repositories.Ticket;
using Lottery.Core.Services.Agent;
using Lottery.Core.Services.Caching.Player;
using Lottery.Core.Services.Odds;
using Lottery.Core.Services.Player;
using Lottery.Core.Services.Pubs;
using Lottery.Core.UnitOfWorks;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace Lottery.Core.Services.Ticket;

public class ProcessMixedService : LotteryBaseService<ProcessMixedService>, IProcessMixedService
{
    private readonly IInMemoryUnitOfWork _inMemoryUnitOfWork;
    private readonly ITicketProcessor _ticketProcessor;
    private readonly IAgentPositionTakingService _agentPositionTakingService;
    private readonly IPlayerSettingService _playerSettingService;
    private readonly IProcessTicketService _processTicketService;
    private readonly IProcessOddsService _processOddsService;
    private readonly IPublishCommonService _publishCommonService;

    public ProcessMixedService(ILogger<ProcessMixedService> logger, IServiceProvider serviceProvider, IConfiguration configuration, IClockService clockService, ILotteryClientContext clientContext, ILotteryUow lotteryUow,
        IInMemoryUnitOfWork inMemoryUnitOfWork,
        ITicketProcessor ticketProcessor,
        IAgentPositionTakingService agentPositionTakingService,
        IPlayerSettingService playerSettingService,
        IProcessTicketService processTicketService,
        IProcessOddsService processOddsService,
        IPublishCommonService publishCommonService) : base(logger, serviceProvider, configuration, clockService, clientContext, lotteryUow)
    {
        _inMemoryUnitOfWork = inMemoryUnitOfWork;
        _ticketProcessor = ticketProcessor;
        _agentPositionTakingService = agentPositionTakingService;
        _playerSettingService = playerSettingService;
        _processTicketService = processTicketService;
        _processOddsService = processOddsService;
        _publishCommonService = publishCommonService;
    }

    public async Task<List<Data.Entities.Ticket>> Process(ProcessMixedTicketModel model, ProcessValidationTicketModel processValidation)
    {
        var noOfNumbers = model.BetKindId.GetNoOfNumbers();
        var enableStats = _ticketProcessor.EnableStats(model.BetKindId);
        var dictSubBetKindIds = _ticketProcessor.GetSubBetKindIds(model.BetKindId);
        var listSubBetKindIds = dictSubBetKindIds.Keys.ToList();

        var betKindInMemoryRepository = _inMemoryUnitOfWork.GetRepository<IBetKindInMemoryRepository>();
        var betKinds = betKindInMemoryRepository.FindBy(f => listSubBetKindIds.Contains(f.Id)).ToList();

        //  Get Player OddsValue, MinBet, MaxBet, MaxPerMatch
        (var subBetKinds, var refreshSettingCache) = await _playerSettingService.GetBetSettings(processValidation.Player.PlayerId, listSubBetKindIds);
        if (subBetKinds.Count == 0) throw new BadRequestException(ErrorCodeHelper.ProcessTicket.CannotReadBetSetting);
        if (refreshSettingCache) await _playerSettingService.BuildSettingByBetKindCache(processValidation.Player.PlayerId, subBetKinds);

        //  Get rate of OddsValue from Company
        var rateOfOddsValue = await _processOddsService.GetMixedRateOfOddsValue(processValidation.Match.MatchId, listSubBetKindIds);

        //  Player Odds by Match, BetKind
        var dictPlayerOdds = await _processTicketService.GetMatchPlayerMixedOddsByBetKind(processValidation.Player.PlayerId, processValidation.Match.MatchId, subBetKinds);

        //  Get Company Odds, Agent Odds
        var agentOddsValue = await _processTicketService.GetAgentMixedOdds(processValidation.BetKind.Id, dictSubBetKindIds.Keys.ToList(), processValidation.Player.SupermasterId, processValidation.Player.MasterId, processValidation.Player.AgentId) ?? throw new BadRequestException(ErrorCodeHelper.ProcessTicket.CannotReadAgentOdds);

        //  Get agent position taking
        var agentPts = await _agentPositionTakingService.GetAgentPositionTakingByAgentIds(listSubBetKindIds, new List<long> { processValidation.Player.SupermasterId, processValidation.Player.MasterId, processValidation.Player.AgentId });

        //  Get Credit
        (var givenCredit, var refreshCreditCache) = await _processTicketService.GetGivenCredit(processValidation.Player.PlayerId);
        if (refreshCreditCache) await _processTicketService.BuildGivenCreditCache(processValidation.Player.PlayerId, givenCredit);

        //  Get Outs
        var outs = await _processTicketService.GetMixedOdds(processValidation.Player.PlayerId, processValidation.Match.MatchId, listSubBetKindIds, model.Numbers);

        var ticketRepository = LotteryUow.GetRepository<ITicketRepository>();
        var correlationId = Guid.NewGuid();
        var numbers = model.Numbers.OrderBy(f => f).ToList();
        var normalizedNumbers = JoinNumbers(numbers, noOfNumbers);
        var totalPayouts = 0m;
        var tickets = new List<Data.Entities.Ticket>();
        //  Stats
        var pointByNumbers = new Dictionary<int, Dictionary<int, decimal>>();   //  Key = BetKindId; Value = Dict<Key = Number; Value = Point>

        var pointByPair = new Dictionary<int, Dictionary<string, decimal>>();   //  Key = BetKindId; Value = Dict<Key = PairNumber; Value = Point>
        var payoutByPair = new Dictionary<int, Dictionary<string, decimal>>();  //  Key = BetKindId; Value = Dict<Key = PairNumber; Value = Payout>
        var realPayoutByPair = new Dictionary<int, Dictionary<string, decimal>>();  //  Key = BetKindId; Value = Dict<Key = PairNumber; Value = Real Payout>
        foreach (var item in dictSubBetKindIds)
        {
            var betKindId = item.Key;
            var noOfElements = item.Value;

            var currentBetKind = betKinds.FirstOrDefault(f => f.Id == betKindId);
            if (currentBetKind == null) continue;

            if (!outs.PointsByMatchAndNumbers.TryGetValue(betKindId, out Dictionary<int, decimal> outsByBetKind)) outsByBetKind = new Dictionary<int, decimal>();

            if (!agentPts.TryGetValue(betKindId, out List<AgentPositionTakingModel> positionTakings)) positionTakings = new List<AgentPositionTakingModel>();
            if (!rateOfOddsValue.TryGetValue(betKindId, out decimal rateValue)) rateValue = 0m;

            if (!pointByNumbers.TryGetValue(betKindId, out Dictionary<int, decimal> valPointByNumbers))
            {
                valPointByNumbers = new Dictionary<int, decimal>();
                pointByNumbers[betKindId] = valPointByNumbers;
            }

            if (!pointByPair.TryGetValue(betKindId, out Dictionary<string, decimal> valPointByPair))
            {
                valPointByPair = new Dictionary<string, decimal>();
                pointByPair[betKindId] = valPointByPair;
            }
            if (!payoutByPair.TryGetValue(betKindId, out Dictionary<string, decimal> valPayoutByPair))
            {
                valPayoutByPair = new Dictionary<string, decimal>();
                payoutByPair[betKindId] = valPayoutByPair;
            }
            if (!realPayoutByPair.TryGetValue(betKindId, out Dictionary<string, decimal> valRealPayoutByPair))
            {
                valRealPayoutByPair = new Dictionary<string, decimal>();
                realPayoutByPair[betKindId] = valRealPayoutByPair;
            }

            var subsets = new List<List<int>>();
            numbers.ToArray().GenerateCombination(noOfElements, subsets);
            if (subsets.Count == 0) continue;
            if (!model.Points.TryGetValue(betKindId, out decimal pointByBetKind)) continue;
            if (!dictPlayerOdds.TryGetValue(betKindId, out decimal playerOddsValue)) continue;

            //  Bet setting
            if (!subBetKinds.TryGetValue(betKindId, out BetSettingModel betSetting)) continue;
            if (pointByBetKind < betSetting.MinBet || pointByBetKind > betSetting.MaxBet) throw new BadRequestException(ErrorCodeHelper.ProcessTicket.PointIsInvalid);

            //  Agent Odds
            if (!agentOddsValue.AgentOdds.TryGetValue(betKindId, out decimal agentOdds)) throw new BadRequestException(ErrorCodeHelper.ProcessTicket.CannotFindOddsOfNumber);
            if (!agentOddsValue.MasterOdds.TryGetValue(betKindId, out decimal masterOdds)) throw new BadRequestException(ErrorCodeHelper.ProcessTicket.CannotFindOddsOfNumber);
            if (!agentOddsValue.SupermasterOdds.TryGetValue(betKindId, out decimal supermasterOdds)) throw new BadRequestException(ErrorCodeHelper.ProcessTicket.CannotFindOddsOfNumber);
            if (!agentOddsValue.CompanyOdds.TryGetValue(betKindId, out decimal companyOdds)) throw new BadRequestException(ErrorCodeHelper.ProcessTicket.CannotFindOddsOfNumber);

            playerOddsValue += rateValue;

            if (subsets.Count == 1)
            {
                var ticket = CreateSingleTicket(correlationId, processValidation, currentBetKind, pointByBetKind, playerOddsValue, normalizedNumbers, positionTakings);

                ticket.AgentOdds = agentOdds;
                ticket.AgentPayout = pointByBetKind * agentOdds;

                ticket.MasterOdds = masterOdds;
                ticket.MasterPayout = pointByBetKind * masterOdds;

                ticket.SupermasterOdds = supermasterOdds;
                ticket.SupermasterPayout = pointByBetKind * supermasterOdds;

                ticket.CompanyOdds = companyOdds;
                ticket.CompanyPayout = pointByBetKind * companyOdds;

                foreach (var number in numbers)
                {
                    if (!outsByBetKind.TryGetValue(number, out decimal outsByMatchAndNumberValue)) outsByMatchAndNumberValue = 0m;
                    if ((outsByMatchAndNumberValue + ticket.Stake) > betSetting.MaxPerNumber) throw new BadRequestException(ErrorCodeHelper.ProcessTicket.MaxPerNumberIsInvalid);

                    valPointByNumbers[number] = ticket.Stake;
                }

                valPointByPair[normalizedNumbers] = pointByBetKind;
                valPayoutByPair[normalizedNumbers] = ticket.PlayerPayout;
                valRealPayoutByPair[normalizedNumbers] = _ticketProcessor.GetRealPayoutForCompany(ticket.PlayerPayout, ticket.SupermasterPt);

                totalPayouts += ticket.PlayerPayout;
                ticketRepository.Add(ticket);
                tickets.Add(ticket);
            }
            else
            {
                //  Set total points
                var dictTotalPointsByNumbers = new Dictionary<int, decimal>();
                foreach (var itemSubsets in subsets)
                {
                    foreach (var itemNumber in itemSubsets)
                    {
                        if (!dictTotalPointsByNumbers.ContainsKey(itemNumber)) dictTotalPointsByNumbers[itemNumber] = pointByBetKind;
                        else dictTotalPointsByNumbers[itemNumber] += pointByBetKind;
                    }
                }
                foreach (var number in dictTotalPointsByNumbers)
                {
                    if (!outsByBetKind.TryGetValue(number.Key, out decimal pointsByMatchAndNumberValue)) pointsByMatchAndNumberValue = 0m;
                    if ((pointsByMatchAndNumberValue + number.Value) > betSetting.MaxPerNumber) throw new BadRequestException(ErrorCodeHelper.ProcessTicket.MaxPerNumberIsInvalid);
                    valPointByNumbers[number.Key] = number.Value;
                }

                var ticket = CreateParentTicket(correlationId, processValidation, betKindId, currentBetKind, playerOddsValue, normalizedNumbers, positionTakings);
                var childTickets = new List<Data.Entities.Ticket>();
                foreach (var itemSubsets in subsets)
                {
                    var subsetNumbers = itemSubsets.OrderBy(f => f).ToList();
                    var normalizedSubsetNumbers = JoinNumbers(subsetNumbers, noOfNumbers);
                    var childTicketItem = CreateChildrenTicket(ticket, currentBetKind, pointByBetKind, playerOddsValue, normalizedSubsetNumbers, positionTakings);

                    childTicketItem.AgentOdds = agentOdds;
                    childTicketItem.AgentPayout = pointByBetKind * agentOdds;

                    childTicketItem.MasterOdds = masterOdds;
                    childTicketItem.MasterPayout = pointByBetKind * masterOdds;

                    childTicketItem.SupermasterOdds = supermasterOdds;
                    childTicketItem.SupermasterPayout = pointByBetKind * supermasterOdds;

                    childTicketItem.CompanyOdds = companyOdds;
                    childTicketItem.CompanyPayout = pointByBetKind * companyOdds;

                    valPointByPair[normalizedSubsetNumbers] = pointByBetKind;
                    valPayoutByPair[normalizedSubsetNumbers] = childTicketItem.PlayerPayout;
                    valRealPayoutByPair[normalizedSubsetNumbers] = _ticketProcessor.GetRealPayoutForCompany(childTicketItem.PlayerPayout, childTicketItem.SupermasterPt);

                    childTickets.Add(childTicketItem);
                }
                ticket.Stake = childTickets.Sum(f => f.Stake);
                ticket.PlayerPayout = childTickets.Sum(f => f.PlayerPayout);
                ticket.AgentPayout = childTickets.Sum(f => f.AgentPayout);
                ticket.MasterPayout = childTickets.Sum(f => f.MasterPayout);
                ticket.SupermasterPayout = childTickets.Sum(f => f.SupermasterPayout);
                ticket.CompanyPayout = childTickets.Sum(f => f.CompanyPayout);

                totalPayouts += ticket.PlayerPayout;

                ticketRepository.Add(ticket);
                ticketRepository.AddRange(childTickets);

                tickets.Add(ticket);
                tickets.AddRange(childTickets);
            }
        }

        if ((outs.OutsByMatch + totalPayouts) > givenCredit) throw new BadRequestException(ErrorCodeHelper.ProcessTicket.GivenCreditIsInvalid);

        await LotteryUow.SaveChangesAsync();

        await _processTicketService.BuildOutsByMatchCache(processValidation.Player.PlayerId, processValidation.Match.MatchId, outs.OutsByMatch + totalPayouts);
        await _processTicketService.BuildPointsByMatchBetKindAndNumbersCache(processValidation.Player.PlayerId, processValidation.Match.MatchId, pointByNumbers);
        if (enableStats)
        {
            await _processTicketService.BuildMixedStatsByMatch(processValidation.Match.MatchId, pointByPair, payoutByPair, realPayoutByPair);
            await _publishCommonService.PublishMixedCompanyPayouts(new Models.Payouts.MixedCompanyPayoutModel
            {
                MatchId = processValidation.Match.MatchId,
                Payouts = realPayoutByPair
            });
        }

        return tickets;
    }

    private Data.Entities.Ticket CreateChildrenTicket(Data.Entities.Ticket parentTicket, BetKindModel betKind, decimal points, decimal playerOddsValue, string normalizedNumbers, List<AgentPositionTakingModel> positionTakings)
    {
        var agentPt = 0m;
        var agentPostionTaking = positionTakings.Find(f => f.AgentId == parentTicket.AgentId);
        if (agentPostionTaking != null) agentPt = agentPostionTaking.PositionTaking;

        var masterPt = 0m;
        var masterPostionTaking = positionTakings.Find(f => f.AgentId == parentTicket.MasterId);
        if (masterPostionTaking != null) masterPt = masterPostionTaking.PositionTaking;

        var supermasterPt = 0m;
        var supermasterPostionTaking = positionTakings.Find(f => f.AgentId == parentTicket.SupermasterId);
        if (supermasterPostionTaking != null) supermasterPt = supermasterPostionTaking.PositionTaking;

        return new Data.Entities.Ticket
        {
            PlayerId = parentTicket.PlayerId,
            AgentId = parentTicket.AgentId,
            MasterId = parentTicket.MasterId,
            SupermasterId = parentTicket.SupermasterId,
            BetKindId = parentTicket.BetKindId,
            SportKindId = parentTicket.SportKindId,
            MatchId = parentTicket.MatchId,
            KickOffTime = parentTicket.KickOffTime,
            RegionId = parentTicket.RegionId,
            ChannelId = parentTicket.ChannelId,
            ChoosenNumbers = normalizedNumbers,
            RewardRate = betKind.Award,
            Stake = points,
            IsLive = parentTicket.IsLive,
            Prize = parentTicket.Prize,
            //  Player
            PlayerOdds = playerOddsValue,
            PlayerPayout = points * playerOddsValue,
            PlayerWinLoss = 0m,
            DraftPlayerWinLoss = 0m,
            //  Agent
            AgentOdds = 0m,
            AgentPayout = 0m,
            AgentWinLoss = 0m,
            DraftAgentWinLoss = 0m,
            AgentPt = agentPt,
            //  Master
            MasterOdds = 0m,
            MasterPayout = 0m,
            MasterWinLoss = 0m,
            DraftMasterWinLoss = 0m,
            MasterPt = masterPt,
            //  Supermaster
            SupermasterOdds = 0m,
            SupermasterPayout = 0m,
            SupermasterWinLoss = 0m,
            DraftSupermasterWinLoss = 0m,
            SupermasterPt = supermasterPt,
            //  Company
            CompanyOdds = 0m,
            CompanyPayout = 0m,
            CompanyWinLoss = 0m,
            DraftCompanyWinLoss = 0m,
            //  State & Others
            State = parentTicket.State,
            CreatedAt = parentTicket.CreatedAt,
            Parent = parentTicket
        };
    }

    private Data.Entities.Ticket CreateParentTicket(Guid correlationId, ProcessValidationTicketModel processValidation, int betKindId, BetKindModel betKind, decimal playerOddsValue, string normalizedNumbers, List<AgentPositionTakingModel> positionTakings)
    {
        var agentPt = 0m;
        var agentPostionTaking = positionTakings.Find(f => f.AgentId == processValidation.Player.AgentId);
        if (agentPostionTaking != null) agentPt = agentPostionTaking.PositionTaking;

        var masterPt = 0m;
        var masterPostionTaking = positionTakings.Find(f => f.AgentId == processValidation.Player.MasterId);
        if (masterPostionTaking != null) masterPt = masterPostionTaking.PositionTaking;

        var supermasterPt = 0m;
        var supermasterPostionTaking = positionTakings.Find(f => f.AgentId == processValidation.Player.SupermasterId);
        if (supermasterPostionTaking != null) supermasterPt = supermasterPostionTaking.PositionTaking;

        return new Data.Entities.Ticket
        {
            PlayerId = processValidation.Player.PlayerId,
            AgentId = processValidation.Player.AgentId,
            MasterId = processValidation.Player.MasterId,
            SupermasterId = processValidation.Player.SupermasterId,
            BetKindId = betKindId,
            SportKindId = SportKind.Lottery.ToInt(),
            MatchId = processValidation.Match.MatchId,
            KickOffTime = processValidation.Match.KickoffTime,
            RegionId = processValidation.BetKind.RegionId,
            ChannelId = processValidation.Channel.Id,
            ChoosenNumbers = normalizedNumbers,
            ShowMore = true,
            CorrelationCode = correlationId,
            RewardRate = betKind.Award,
            Stake = 0m,
            IsLive = processValidation.Metadata.IsLive,
            Prize = processValidation.Metadata.Prize,
            Position = processValidation.Metadata.Position,
            //  Player
            PlayerOdds = playerOddsValue,
            PlayerPayout = 0m,
            PlayerWinLoss = 0m,
            DraftPlayerWinLoss = 0m,
            //  Agent
            AgentOdds = 0m,
            AgentPayout = 0m,
            AgentWinLoss = 0m,
            DraftAgentWinLoss = 0m,
            AgentPt = agentPt,
            //  Master
            MasterOdds = 0m,
            MasterPayout = 0m,
            MasterWinLoss = 0m,
            DraftMasterWinLoss = 0m,
            MasterPt = masterPt,
            //  Supermaster
            SupermasterOdds = 0m,
            SupermasterPayout = 0m,
            SupermasterWinLoss = 0m,
            DraftSupermasterWinLoss = 0m,
            SupermasterPt = supermasterPt,
            //  Company
            CompanyOdds = 0m,
            CompanyPayout = 0m,
            CompanyWinLoss = 0m,
            DraftCompanyWinLoss = 0m,
            //  State & Others
            State = TicketState.Waiting.ToInt(),
            IpAddress = processValidation.ClientInformation.IpAddress,
            UserAgent = processValidation.ClientInformation.UserAgent,
            Platform = processValidation.ClientInformation.Platform,
            CreatedAt = ClockService.GetUtcNow()
        };
    }

    private Data.Entities.Ticket CreateSingleTicket(Guid correlationId, ProcessValidationTicketModel processValidation, BetKindModel betKind, decimal points, decimal playerOddsValue, string normalizedNumbers, List<AgentPositionTakingModel> positionTakings)
    {
        //  PT
        var agentPt = 0m;
        var agentPostionTaking = positionTakings.Find(f => f.AgentId == processValidation.Player.AgentId);
        if (agentPostionTaking != null) agentPt = agentPostionTaking.PositionTaking;

        var masterPt = 0m;
        var masterPostionTaking = positionTakings.Find(f => f.AgentId == processValidation.Player.MasterId);
        if (masterPostionTaking != null) masterPt = masterPostionTaking.PositionTaking;

        var supermasterPt = 0m;
        var supermasterPostionTaking = positionTakings.Find(f => f.AgentId == processValidation.Player.SupermasterId);
        if (supermasterPostionTaking != null) supermasterPt = supermasterPostionTaking.PositionTaking;

        return new Data.Entities.Ticket
        {
            PlayerId = processValidation.Player.PlayerId,
            AgentId = processValidation.Player.AgentId,
            MasterId = processValidation.Player.MasterId,
            SupermasterId = processValidation.Player.SupermasterId,
            BetKindId = betKind.Id,
            SportKindId = SportKind.Lottery.ToInt(),
            MatchId = processValidation.Match.MatchId,
            KickOffTime = processValidation.Match.KickoffTime,
            RegionId = processValidation.BetKind.RegionId,
            ChannelId = processValidation.Channel.Id,
            ChoosenNumbers = normalizedNumbers,
            CorrelationCode = correlationId,
            RewardRate = betKind.Award,
            Stake = points,
            IsLive = processValidation.Metadata.IsLive,
            Prize = processValidation.Metadata.Prize,
            Position = processValidation.Metadata.Position,
            //  Player
            PlayerOdds = playerOddsValue,
            PlayerPayout = points * playerOddsValue,
            PlayerWinLoss = 0m,
            DraftPlayerWinLoss = 0m,
            //  Agent
            AgentOdds = 0m,
            AgentPayout = 0m,
            AgentWinLoss = 0m,
            DraftAgentWinLoss = 0m,
            AgentPt = agentPt,
            //  Master
            MasterOdds = 0m,
            MasterPayout = 0m,
            MasterWinLoss = 0m,
            DraftMasterWinLoss = 0m,
            MasterPt = masterPt,
            //  Supermaster
            SupermasterOdds = 0m,
            SupermasterPayout = 0m,
            SupermasterWinLoss = 0m,
            DraftSupermasterWinLoss = 0m,
            SupermasterPt = supermasterPt,
            //  Company
            CompanyOdds = 0m,
            CompanyPayout = 0m,
            CompanyWinLoss = 0m,
            DraftCompanyWinLoss = 0m,
            //  State & Others
            State = TicketState.Waiting.ToInt(),
            IpAddress = processValidation.ClientInformation.IpAddress,
            UserAgent = processValidation.ClientInformation.UserAgent,
            Platform = processValidation.ClientInformation.Platform,
            CreatedAt = ClockService.GetUtcNow()
        };
    }

    private string JoinNumbers(List<int> numbers, int noOfNumbers)
    {
        return string.Join(", ", numbers.Select(f => f.NormalizeNumber(noOfNumbers))).Trim();
    }
}