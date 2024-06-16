using HnMicro.Core.Helpers;
using HnMicro.Framework.Enums;
using HnMicro.Framework.Exceptions;
using HnMicro.Framework.Services;
using HnMicro.Modules.InMemory.UnitOfWorks;
using Lottery.Core.Contexts;
using Lottery.Core.Enums;
using Lottery.Core.Helpers;
using Lottery.Core.InMemory.BetKind;
using Lottery.Core.InMemory.Channel;
using Lottery.Core.InMemory.Prize;
using Lottery.Core.InMemory.Ticket;
using Lottery.Core.Models.BetKind;
using Lottery.Core.Models.MatchResult;
using Lottery.Core.Models.PositionTakings;
using Lottery.Core.Models.Setting;
using Lottery.Core.Models.Ticket;
using Lottery.Core.Models.Ticket.Process;
using Lottery.Core.Repositories.Match;
using Lottery.Core.Repositories.MatchResult;
using Lottery.Core.Repositories.Ticket;
using Lottery.Core.Services.Agent;
using Lottery.Core.Services.Caching.Player;
using Lottery.Core.Services.Odds;
using Lottery.Core.Services.Player;
using Lottery.Core.UnitOfWorks;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace Lottery.Core.Services.Ticket;

public class TicketService : LotteryBaseService<TicketService>, ITicketService
{
    private readonly IInMemoryUnitOfWork _inMemoryUnitOfWork;
    private readonly ITicketProcessor _ticketProcessor;
    private readonly IAgentPositionTakingService _agentPositionTakingService;
    private readonly IPlayerService _playerService;
    private readonly IPlayerSettingService _playerSettingService;
    private readonly IProcessTicketService _processTicketService;
    private readonly IProcessOddsService _processOddsService;

    public TicketService(ILogger<TicketService> logger, IServiceProvider serviceProvider, IConfiguration configuration, IClockService clockService, ILotteryClientContext clientContext, ILotteryUow lotteryUow,
        IInMemoryUnitOfWork inMemoryUnitOfWork,
        ITicketProcessor validationTicketHandler,
        IAgentPositionTakingService agentPositionTakingService,
        IPlayerService playerService,
        IPlayerSettingService playerSettingService,
        IProcessTicketService processTicketService,
        IProcessOddsService processOddsService) : base(logger, serviceProvider, configuration, clockService, clientContext, lotteryUow)
    {
        _inMemoryUnitOfWork = inMemoryUnitOfWork;
        _ticketProcessor = validationTicketHandler;
        _agentPositionTakingService = agentPositionTakingService;
        _playerService = playerService;
        _playerSettingService = playerSettingService;
        _processTicketService = processTicketService;
        _processOddsService = processOddsService;
    }

    public async Task Process(ProcessTicketModel model)
    {
        var processValidation = await InternalProcess(model.BetKindId, model.MatchId);

        //  Validation BetKind
        var errCode = _ticketProcessor.Valid(model, processValidation.Metadata);
        if (errCode < 0) throw new BadRequestException(errCode);

        var enableStats = _ticketProcessor.EnableStats(model.BetKindId);

        //  Get Player OddsValue, MinBet, MaxBet, MaxPerMatch
        (var setting, var refreshSettingCache) = await _playerSettingService.GetBetSettings(processValidation.Player.PlayerId, processValidation.BetKind.Id);
        if (setting == null) throw new BadRequestException(ErrorCodeHelper.ProcessTicket.CannotReadBetSetting);
        if (refreshSettingCache) await _playerSettingService.BuildSettingByBetKindCache(processValidation.Player.PlayerId, processValidation.BetKind.Id, setting);

        //  Get rate of OddsValue from Company
        var rateOfOddsValue = await _processOddsService.GetRateOfOddsValue(processValidation.Match.MatchId, new List<int> { model.BetKindId });
        if (!rateOfOddsValue.TryGetValue(model.BetKindId, out Dictionary<int, decimal> rateOfOddsValueByBetKind)) rateOfOddsValueByBetKind = new Dictionary<int, decimal>();

        //  Player Odds by Match, BetKind and Numbers
        var dictPlayerOdds = await _processTicketService.GetMatchPlayerOddsByBetKindAndNumbers(processValidation.Player.PlayerId, setting.OddsValue, processValidation.Match.MatchId, processValidation.BetKind.Id, model.Numbers.Select(f => f.Number).ToList());

        //  Get Company Odds, Agent Odds
        var agentOddsValue = await _processTicketService.GetAgentOdds(processValidation.BetKind.Id, processValidation.Player.SupermasterId, processValidation.Player.MasterId, processValidation.Player.AgentId) ?? throw new BadRequestException(ErrorCodeHelper.ProcessTicket.CannotReadAgentOdds);

        //  Get agent position taking
        var agentPts = await _agentPositionTakingService.GetAgentPositionTakingByAgentIds(new List<int> { processValidation.BetKind.Id }, new List<long> { processValidation.Player.SupermasterId, processValidation.Player.MasterId, processValidation.Player.AgentId });
        if (!agentPts.TryGetValue(processValidation.BetKind.Id, out List<AgentPositionTakingModel> positionTaking)) positionTaking = new List<AgentPositionTakingModel>();

        //  Get Credit
        (var givenCredit, var refreshCreditCache) = await _processTicketService.GetGivenCredit(processValidation.Player.PlayerId);
        if (refreshCreditCache) await _processTicketService.BuildGivenCreditCache(processValidation.Player.PlayerId, givenCredit);

        //  Get Outs
        var outs = await _processTicketService.GetOuts(processValidation.Player.PlayerId, processValidation.Match.MatchId, model.Numbers.Select(f => f.Number).ToList());

        var ticketRepository = LotteryUow.GetRepository<ITicketRepository>();
        var ticket = new Data.Entities.Ticket
        {
            PlayerId = processValidation.Player.PlayerId,
            AgentId = processValidation.Player.AgentId,
            MasterId = processValidation.Player.MasterId,
            SupermasterId = processValidation.Player.SupermasterId,
            BetKindId = processValidation.BetKind.Id,
            SportKindId = SportKind.Lottery.ToInt(),
            MatchId = processValidation.Match.MatchId,
            KickOffTime = processValidation.Match.KickOffTime,
            RegionId = processValidation.BetKind.RegionId,
            ChannelId = processValidation.Channel.Id,
            ChoosenNumbers = string.Empty,

            RewardRate = processValidation.BetKind.Award,
            Stake = 0m,
            PlayerOdds = 0m,
            PlayerPayout = 0m,
            PlayerWinLoss = 0m,

            AgentOdds = 0m,
            AgentPayout = 0m,
            AgentWinLoss = 0m,
            AgentCommission = 0m,
            AgentPt = 0m,

            MasterOdds = 0m,
            MasterPayout = 0m,
            MasterWinLoss = 0m,
            MasterCommission = 0m,
            MasterPt = 0m,

            SupermasterOdds = 0m,
            SupermasterPayout = 0m,
            SupermasterWinLoss = 0m,
            SupermasterCommission = 0m,
            SupermasterPt = 0m,

            CompanyOdds = 0m,
            CompanyPayout = 0m,
            CompanyWinLoss = 0m,

            State = TicketState.Waiting.ToInt(),
            IsLive = processValidation.Metadata.IsLive,
            Prize = processValidation.Metadata.Prize,
            Position = processValidation.Metadata.Position,

            IpAddress = processValidation.ClientInformation.IpAddress,
            UserAgent = processValidation.ClientInformation.UserAgent,
            Platform = processValidation.ClientInformation.Platform,
            CreatedAt = ClockService.GetUtcNow()
        };

        //  Set PT
        var agentPostionTaking = positionTaking.Find(f => f.AgentId == ticket.AgentId);
        if (agentPostionTaking != null) ticket.AgentPt = agentPostionTaking.PositionTaking;

        var masterPostionTaking = positionTaking.Find(f => f.AgentId == ticket.MasterId);
        if (masterPostionTaking != null) ticket.MasterPt = masterPostionTaking.PositionTaking;

        var supermasterPostionTaking = positionTaking.Find(f => f.AgentId == ticket.SupermasterId);
        if (supermasterPostionTaking != null) ticket.SupermasterPt = supermasterPostionTaking.PositionTaking;

        var totalStake = 0m;
        var totalPlayerPayout = 0m;
        var totalAgentPayout = 0m;
        var totalMasterPayout = 0m;
        var totalSupermasterPayout = 0m;
        var totalCompanyPayout = 0m;
        var childTickets = new List<Data.Entities.Ticket>();
        var pointByNumbers = new Dictionary<int, decimal>();
        var payoutByNumbers = new Dictionary<int, decimal>();
        var oddsValueByNumbers = new Dictionary<int, decimal>();
        if (model.Numbers.Count == 1)
        {
            var thisNumber = model.Numbers.First();
            ticket.ChoosenNumbers = thisNumber.Number.NormalizeNumber();
            ticket.ShowMore = false;

            //  Rate of odds value
            if (!rateOfOddsValueByBetKind.TryGetValue(thisNumber.Number, out decimal rateValue)) rateValue = 0m;

            //  Player
            if (!dictPlayerOdds.TryGetValue(thisNumber.Number, out decimal playerOddsValue)) throw new BadRequestException(ErrorCodeHelper.ProcessTicket.CannotFindOddsOfNumber);
            ticket.PlayerOdds = playerOddsValue + rateValue;
            ticket.PlayerPayout = _ticketProcessor.GetPayoutByNumber(processValidation.BetKind, thisNumber.Point, ticket.PlayerOdds.Value);

            //  Agent
            if (!agentOddsValue.AgentOdds.TryGetValue(thisNumber.Number, out decimal aOddsValue)) throw new BadRequestException(ErrorCodeHelper.ProcessTicket.CannotFindOddsOfNumber);
            ticket.AgentOdds = aOddsValue;
            ticket.AgentPayout = _ticketProcessor.GetPayoutByNumber(processValidation.BetKind, thisNumber.Point, aOddsValue);

            //  Master
            if (!agentOddsValue.MasterOdds.TryGetValue(thisNumber.Number, out decimal mOddsValue)) throw new BadRequestException(ErrorCodeHelper.ProcessTicket.CannotFindOddsOfNumber);
            ticket.MasterOdds = mOddsValue;
            ticket.MasterPayout = _ticketProcessor.GetPayoutByNumber(processValidation.BetKind, thisNumber.Point, mOddsValue);

            //  Supermaster
            if (!agentOddsValue.SupermasterOdds.TryGetValue(thisNumber.Number, out decimal sOddsValue)) throw new BadRequestException(ErrorCodeHelper.ProcessTicket.CannotFindOddsOfNumber);
            ticket.SupermasterOdds = sOddsValue;
            ticket.SupermasterPayout = _ticketProcessor.GetPayoutByNumber(processValidation.BetKind, thisNumber.Point, sOddsValue);

            //  Company
            if (!agentOddsValue.CompanyOdds.TryGetValue(thisNumber.Number, out decimal cOddsValue)) throw new BadRequestException(ErrorCodeHelper.ProcessTicket.CannotFindOddsOfNumber);
            ticket.CompanyOdds = cOddsValue;
            ticket.CompanyPayout = _ticketProcessor.GetPayoutByNumber(processValidation.BetKind, thisNumber.Point, cOddsValue);

            ticket.Stake = thisNumber.Point;

            totalStake += ticket.Stake;
            totalPlayerPayout += ticket.PlayerPayout;
            totalAgentPayout += ticket.AgentPayout;
            totalMasterPayout += ticket.MasterPayout;
            totalSupermasterPayout += ticket.SupermasterPayout;
            totalCompanyPayout += ticket.CompanyPayout;

            pointByNumbers[thisNumber.Number] = ticket.Stake;
            payoutByNumbers[thisNumber.Number] = ticket.PlayerPayout;
            oddsValueByNumbers[thisNumber.Number] = ticket.PlayerOdds.Value;

            if (thisNumber.Point < setting.MinBet || thisNumber.Point > setting.MaxBet) throw new BadRequestException(ErrorCodeHelper.ProcessTicket.PointIsInvalid);
            if (!outs.PointsByMatchAndNumbers.TryGetValue(thisNumber.Number, out decimal pointsByMatchAndNumberValue)) pointsByMatchAndNumberValue = 0m;
            if ((pointsByMatchAndNumberValue + totalStake) > setting.MaxPerNumber) throw new BadRequestException(ErrorCodeHelper.ProcessTicket.MaxPerNumberIsInvalid, thisNumber.Number.NormalizeNumber());
            if ((outs.OutsByMatch + totalPlayerPayout) > givenCredit) throw new BadRequestException(ErrorCodeHelper.ProcessTicket.GivenCreditIsInvalid);
        }
        else
        {
            var numbers = model.Numbers.OrderBy(f => f.Number).ToList();
            var normalizedNumbers = new List<string>();
            foreach (var item in numbers)
            {
                if (item.Point < setting.MinBet || item.Point > setting.MaxBet) throw new BadRequestException(ErrorCodeHelper.ProcessTicket.PointIsInvalid);
                //  Rate of odds value
                if (!rateOfOddsValueByBetKind.TryGetValue(item.Number, out decimal rateValue)) rateValue = 0m;
                //  Player
                if (!dictPlayerOdds.TryGetValue(item.Number, out decimal playerOddsValue)) throw new BadRequestException(ErrorCodeHelper.ProcessTicket.CannotFindOddsOfNumber);
                //  Agent
                if (!agentOddsValue.AgentOdds.TryGetValue(item.Number, out decimal aOddsValue)) throw new BadRequestException(ErrorCodeHelper.ProcessTicket.CannotFindOddsOfNumber);
                //  Master
                if (!agentOddsValue.MasterOdds.TryGetValue(item.Number, out decimal mOddsValue)) throw new BadRequestException(ErrorCodeHelper.ProcessTicket.CannotFindOddsOfNumber);
                //  Supermaster
                if (!agentOddsValue.SupermasterOdds.TryGetValue(item.Number, out decimal sOddsValue)) throw new BadRequestException(ErrorCodeHelper.ProcessTicket.CannotFindOddsOfNumber);
                //  Company
                if (!agentOddsValue.CompanyOdds.TryGetValue(item.Number, out decimal cOddsValue)) throw new BadRequestException(ErrorCodeHelper.ProcessTicket.CannotFindOddsOfNumber);

                var normalizeNumber = item.Number.NormalizeNumber();

                var playerPayout = _ticketProcessor.GetPayoutByNumber(processValidation.BetKind, item.Point, playerOddsValue + rateValue);
                var agentPayout = _ticketProcessor.GetPayoutByNumber(processValidation.BetKind, item.Point, aOddsValue);
                var masterPayout = _ticketProcessor.GetPayoutByNumber(processValidation.BetKind, item.Point, mOddsValue);
                var supermasterPayout = _ticketProcessor.GetPayoutByNumber(processValidation.BetKind, item.Point, sOddsValue);
                var companyPayout = _ticketProcessor.GetPayoutByNumber(processValidation.BetKind, item.Point, cOddsValue);

                pointByNumbers[item.Number] = item.Point;
                payoutByNumbers[item.Number] = playerPayout;
                oddsValueByNumbers[item.Number] = playerOddsValue + rateValue;

                totalStake += item.Point;
                totalPlayerPayout += playerPayout;
                totalAgentPayout += agentPayout;
                totalMasterPayout += masterPayout;
                totalSupermasterPayout += supermasterPayout;
                totalCompanyPayout += companyPayout;

                if (!outs.PointsByMatchAndNumbers.TryGetValue(item.Number, out decimal pointsByMatchAndNumberValue)) pointsByMatchAndNumberValue = 0m;
                if ((pointsByMatchAndNumberValue + item.Point) > setting.MaxPerNumber) throw new BadRequestException(ErrorCodeHelper.ProcessTicket.MaxPerNumberIsInvalid, normalizeNumber);

                normalizedNumbers.Add(normalizeNumber);

                childTickets.Add(new Data.Entities.Ticket
                {
                    PlayerId = processValidation.Player.PlayerId,
                    AgentId = processValidation.Player.AgentId,
                    MasterId = processValidation.Player.MasterId,
                    SupermasterId = processValidation.Player.SupermasterId,
                    BetKindId = ticket.BetKindId,
                    SportKindId = ticket.SportKindId,
                    MatchId = ticket.MatchId,
                    KickOffTime = ticket.KickOffTime,
                    RegionId = ticket.RegionId,
                    ChannelId = ticket.ChannelId,
                    ChoosenNumbers = normalizeNumber,
                    RewardRate = ticket.RewardRate,
                    Stake = item.Point,

                    PlayerOdds = playerOddsValue + rateValue,
                    PlayerPayout = playerPayout,
                    PlayerWinLoss = ticket.PlayerWinLoss,

                    AgentOdds = aOddsValue,
                    AgentPayout = agentPayout,
                    AgentWinLoss = ticket.AgentWinLoss,
                    AgentPt = ticket.AgentPt,

                    MasterOdds = mOddsValue,
                    MasterPayout = masterPayout,
                    MasterWinLoss = ticket.MasterWinLoss,
                    MasterPt = ticket.MasterPt,

                    SupermasterOdds = sOddsValue,
                    SupermasterPayout = supermasterPayout,
                    SupermasterWinLoss = ticket.SupermasterWinLoss,
                    SupermasterPt = ticket.SupermasterPt,

                    CompanyOdds = cOddsValue,
                    CompanyPayout = companyPayout,
                    CompanyWinLoss = ticket.CompanyWinLoss,

                    State = ticket.State,
                    CreatedAt = ticket.CreatedAt,

                    IsLive = ticket.IsLive,
                    Prize = ticket.Prize,

                    Parent = ticket
                });
            }

            ticket.ChoosenNumbers = string.Join(", ", normalizedNumbers);
            ticket.Stake = totalStake;
            ticket.PlayerOdds = _ticketProcessor.GetPlayerOdds(model.BetKindId, oddsValueByNumbers);
            ticket.PlayerPayout = totalPlayerPayout;
            ticket.AgentPayout = totalAgentPayout;
            ticket.MasterPayout = totalMasterPayout;
            ticket.SupermasterPayout = totalSupermasterPayout;
            ticket.CompanyPayout = totalCompanyPayout;
            ticket.ShowMore = true;

            if ((outs.OutsByMatch + totalPlayerPayout) > givenCredit) throw new BadRequestException(ErrorCodeHelper.ProcessTicket.GivenCreditIsInvalid);
        }

        ticketRepository.Add(ticket);
        if (childTickets.Count > 0) ticketRepository.AddRange(childTickets);

        await LotteryUow.SaveChangesAsync();

        await _processTicketService.BuildOutsByMatchCache(processValidation.Player.PlayerId, processValidation.Match.MatchId, outs.OutsByMatch + totalPlayerPayout);
        await _processTicketService.BuildOutsByMatchAndNumbersCache(processValidation.Player.PlayerId, processValidation.Match.MatchId, outs.PointsByMatchAndNumbers, pointByNumbers);
        if (enableStats) await _processTicketService.BuildStatsByMatchBetKindAndNumbers(processValidation.Match.MatchId, processValidation.BetKind.Id, pointByNumbers, payoutByNumbers);

        AddToAcceptedScanService(ticket, childTickets);
    }

    private void AddToAcceptedScanService(Data.Entities.Ticket ticket, List<Data.Entities.Ticket> childTickets)
    {
        var ticketInMemoryRepository = _inMemoryUnitOfWork.GetRepository<ITicketInMemoryRepository>();
        ticketInMemoryRepository.Add(new TicketModel
        {
            TicketId = ticket.TicketId,
            CreatedAt = ticket.CreatedAt,
            Children = childTickets.Select(f => f.TicketId).ToList()
        });
    }

    public async Task ProcessMixed(ProcessMixedTicketModel model)
    {
        var processValidation = await InternalProcess(model.BetKindId, model.MatchId);

        //  Validation BetKind
        var errCode = _ticketProcessor.ValidMixed(model, processValidation.Metadata);
        if (errCode < 0) throw new BadRequestException(errCode);

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
        var rateOfOddsValue = await _processOddsService.GetRateOfOddsValue(processValidation.Match.MatchId, listSubBetKindIds);

        //  Player Odds by Match, BetKind
        var dictPlayerOdds = await _processTicketService.GetMatchPlayerMixedOddsByBetKind(processValidation.Player.PlayerId, processValidation.Match.MatchId, model.BetKindId, subBetKinds);

        //  Get Company Odds, Agent Odds
        var agentOddsValue = await _processTicketService.GetAgentMixedOdds(processValidation.BetKind.Id, dictSubBetKindIds.Keys.ToList(), processValidation.Player.SupermasterId, processValidation.Player.MasterId, processValidation.Player.AgentId) ?? throw new BadRequestException(ErrorCodeHelper.ProcessTicket.CannotReadAgentOdds);

        //  Get agent position taking
        var agentPts = await _agentPositionTakingService.GetAgentPositionTakingByAgentIds(listSubBetKindIds, new List<long> { processValidation.Player.SupermasterId, processValidation.Player.MasterId, processValidation.Player.AgentId });

        //  Get Credit
        (var givenCredit, var refreshCreditCache) = await _processTicketService.GetGivenCredit(processValidation.Player.PlayerId);
        if (refreshCreditCache) await _processTicketService.BuildGivenCreditCache(processValidation.Player.PlayerId, givenCredit);

        //  Get Outs
        var outs = await _processTicketService.GetOuts(processValidation.Player.PlayerId, processValidation.Match.MatchId, model.Numbers);

        var ticketRepository = LotteryUow.GetRepository<ITicketRepository>();
        var correlationId = Guid.NewGuid();
        var numbers = model.Numbers.OrderBy(f => f).ToList();
        var normalizedNumbers = numbers.Select(f => f.NormalizeNumber()).ToList();
        var totalPayouts = 0m;
        var tickets = new List<Data.Entities.Ticket>();
        foreach (var item in dictSubBetKindIds)
        {
            var betKindId = item.Key;
            var noOfElements = item.Value;

            var currentBetKind = betKinds.FirstOrDefault(f => f.Id == betKindId);
            if (currentBetKind == null) continue;

            if (!agentPts.TryGetValue(betKindId, out List<AgentPositionTakingModel> positionTakings)) positionTakings = new List<AgentPositionTakingModel>();
            if (!rateOfOddsValue.TryGetValue(betKindId, out Dictionary<int, decimal> rateValue)) rateValue = new Dictionary<int, decimal>();

            var subsets = new List<List<int>>();
            numbers.ToArray().GenerateCombination(noOfElements, subsets);
            if (subsets.Count == 0) continue;
            if (!model.Points.TryGetValue(betKindId, out decimal pointByBetKind)) continue;
            if (!dictPlayerOdds.TryGetValue(betKindId, out decimal playerOddsValue)) continue;

            //  Bet setting
            if (!subBetKinds.TryGetValue(betKindId, out BetSettingModel betSetting)) continue;
            if (pointByBetKind < betSetting.MinBet || pointByBetKind > betSetting.MaxBet) throw new BadRequestException(ErrorCodeHelper.ProcessTicket.PointIsInvalid);

            if (subsets.Count == 1)
            {
                var ticket = CreateSingleTicket(correlationId, processValidation, currentBetKind, pointByBetKind, playerOddsValue, normalizedNumbers);
                foreach (var number in numbers)
                {
                    if (!outs.PointsByMatchAndNumbers.TryGetValue(number, out decimal outsByMatchAndNumberValue)) outsByMatchAndNumberValue = 0m;
                    if ((outsByMatchAndNumberValue + ticket.Stake) > betSetting.MaxPerNumber) throw new BadRequestException(ErrorCodeHelper.ProcessTicket.MaxPerNumberIsInvalid);
                }
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
                    if (!outs.PointsByMatchAndNumbers.TryGetValue(number.Key, out decimal pointsByMatchAndNumberValue)) pointsByMatchAndNumberValue = 0m;
                    if ((pointsByMatchAndNumberValue + number.Value) > betSetting.MaxPerNumber) throw new BadRequestException(ErrorCodeHelper.ProcessTicket.MaxPerNumberIsInvalid);
                }

                var ticket = CreateParentTicket(correlationId, processValidation, betKindId, currentBetKind, playerOddsValue, normalizedNumbers);
                var childTickets = new List<Data.Entities.Ticket>();
                foreach (var itemSubsets in subsets)
                {
                    var subsetNumbers = itemSubsets.OrderBy(f => f).ToList();
                    var normalizedSubsetNumbers = subsetNumbers.Select(f => f.NormalizeNumber()).ToList();
                    childTickets.Add(CreateChildrenTicket(ticket, currentBetKind, pointByBetKind, playerOddsValue, normalizedSubsetNumbers));
                }
                var subTotalPoints = childTickets.Sum(f => f.Stake);
                var subTotalPayouts = childTickets.Sum(f => f.PlayerPayout);

                ticket.Stake = subTotalPoints;
                ticket.PlayerPayout = subTotalPayouts;

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

        //  TODO Update again...
        //await _processTicketService.BuildOutsByMatchAndNumbersCache(processValidation.Player.PlayerId, processValidation.Match.MatchId, outs.OutsByMatchAndNumbers, payoutByNumbers);
        //if (enableStats) await _processTicketService.BuildStatsByMatchAndNumbers(processValidation.Match.MatchId, processValidation.BetKind.Id, pointByNumbers, payoutByNumbers);

        AddToAcceptedScanMixedService(tickets);
    }

    private void AddToAcceptedScanMixedService(List<Data.Entities.Ticket> tickets)
    {
        var ticketInMemoryRepository = _inMemoryUnitOfWork.GetRepository<ITicketInMemoryRepository>();
        ticketInMemoryRepository.AddRange(tickets.Where(f => !f.ParentId.HasValue).Select(f => new TicketModel
        {
            TicketId = f.TicketId,
            CreatedAt = f.CreatedAt,
            Children = tickets.Where(f1 => f1.ParentId == f.TicketId).Select(f => f.TicketId).ToList()
        }).ToList());
    }

    private Data.Entities.Ticket CreateChildrenTicket(Data.Entities.Ticket parentTicket, BetKindModel betKind, decimal points, decimal playerOddsValue, List<string> normalizedNumbers)
    {
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
            ChoosenNumbers = JoinNumbers(normalizedNumbers),
            RewardRate = betKind.Award,
            Stake = points,
            IsLive = parentTicket.IsLive,
            Prize = parentTicket.Prize,
            //  Player
            PlayerOdds = playerOddsValue,
            PlayerPayout = points * playerOddsValue,
            PlayerWinLoss = 0m,
            //  Agent
            AgentOdds = 0m,
            AgentPayout = 0m,
            AgentWinLoss = 0m,
            //  Master
            MasterOdds = 0m,
            MasterPayout = 0m,
            MasterWinLoss = 0m,
            //  Supermaster
            SupermasterOdds = 0m,
            SupermasterPayout = 0m,
            SupermasterWinLoss = 0m,
            //  Company
            CompanyOdds = 0m,
            CompanyPayout = 0m,
            CompanyWinLoss = 0m,
            //  State & Others
            State = parentTicket.State,
            CreatedAt = parentTicket.CreatedAt,
            Parent = parentTicket
        };
    }

    private Data.Entities.Ticket CreateParentTicket(Guid correlationId, ProcessValidationTicketModel processValidation, int betKindId, BetKindModel betKind, decimal playerOddsValue, List<string> normalizedNumbers)
    {
        return new Data.Entities.Ticket
        {
            PlayerId = processValidation.Player.PlayerId,
            AgentId = processValidation.Player.AgentId,
            MasterId = processValidation.Player.MasterId,
            SupermasterId = processValidation.Player.SupermasterId,
            BetKindId = betKindId,
            SportKindId = SportKind.Lottery.ToInt(),
            MatchId = processValidation.Match.MatchId,
            KickOffTime = processValidation.Match.KickOffTime,
            RegionId = processValidation.BetKind.RegionId,
            ChannelId = processValidation.Channel.Id,
            ChoosenNumbers = JoinNumbers(normalizedNumbers),
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
            //  Agent
            AgentOdds = 0m,
            AgentPayout = 0m,
            AgentWinLoss = 0m,
            //  Master
            MasterOdds = 0m,
            MasterPayout = 0m,
            MasterWinLoss = 0m,
            //  Supermaster
            SupermasterOdds = 0m,
            SupermasterPayout = 0m,
            SupermasterWinLoss = 0m,
            //  Company
            CompanyOdds = 0m,
            CompanyPayout = 0m,
            CompanyWinLoss = 0m,
            //  State & Others
            State = TicketState.Waiting.ToInt(),
            IpAddress = processValidation.ClientInformation.IpAddress,
            UserAgent = processValidation.ClientInformation.UserAgent,
            Platform = processValidation.ClientInformation.Platform,
            CreatedAt = ClockService.GetUtcNow()
        };
    }

    private Data.Entities.Ticket CreateSingleTicket(Guid correlationId, ProcessValidationTicketModel processValidation, BetKindModel betKind, decimal points, decimal playerOddsValue, List<string> normalizedNumbers)
    {
        return new Data.Entities.Ticket
        {
            PlayerId = processValidation.Player.PlayerId,
            AgentId = processValidation.Player.AgentId,
            MasterId = processValidation.Player.MasterId,
            SupermasterId = processValidation.Player.SupermasterId,
            BetKindId = betKind.Id,
            SportKindId = SportKind.Lottery.ToInt(),
            MatchId = processValidation.Match.MatchId,
            KickOffTime = processValidation.Match.KickOffTime,
            RegionId = processValidation.BetKind.RegionId,
            ChannelId = processValidation.Channel.Id,
            ChoosenNumbers = JoinNumbers(normalizedNumbers),
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
            //  Agent
            AgentOdds = 0m,
            AgentPayout = 0m,
            AgentWinLoss = 0m,
            AgentPt = 0m,
            //  Master
            MasterOdds = 0m,
            MasterPayout = 0m,
            MasterWinLoss = 0m,
            //  Supermaster
            SupermasterOdds = 0m,
            SupermasterPayout = 0m,
            SupermasterWinLoss = 0m,
            //  Company
            CompanyOdds = 0m,
            CompanyPayout = 0m,
            CompanyWinLoss = 0m,
            //  State & Others
            State = TicketState.Waiting.ToInt(),
            IpAddress = processValidation.ClientInformation.IpAddress,
            UserAgent = processValidation.ClientInformation.UserAgent,
            Platform = processValidation.ClientInformation.Platform,
            CreatedAt = ClockService.GetUtcNow()
        };
    }

    private async Task<ProcessValidationTicketModel> InternalProcess(int betKindId, long matchId)
    {
        //  Check auth
        var clientInformation = ClientContext.GetClientInformation();
        var player = ClientContext.Player;
        if (clientInformation == null || player == null || player.PlayerId <= 0L) throw new UnauthorizedException();

        //  Player information
        var playerInfo = await _playerService.GetPlayer(player.PlayerId);
        if (playerInfo == null) throw new UnauthorizedException();
        if (playerInfo.Lock) throw new UnauthorizedException();
        if (playerInfo.State.IsSuspended() || (playerInfo.ParentState != null && playerInfo.ParentState.Value.IsSuspended())) throw new BadRequestException(ErrorCodeHelper.ProcessTicket.PlayerIsSuspended);
        if (playerInfo.State.IsClosed() || (playerInfo.ParentState != null && playerInfo.ParentState.Value.IsClosed())) throw new BadRequestException(ErrorCodeHelper.ProcessTicket.PlayerIsClosed);

        //  Check BetKindId
        var betKindRepository = _inMemoryUnitOfWork.GetRepository<IBetKindInMemoryRepository>();
        var betKind = betKindRepository.FindById(betKindId) ?? throw new NotFoundException();
        if (!betKind.Enabled) throw new BadRequestException(ErrorCodeHelper.ProcessTicket.TheBetKindDoesNotAllowProcessing);

        //  Check MatchId
        var matchRepository = LotteryUow.GetRepository<IMatchRepository>();
        var match = await matchRepository.FindByIdAsync(matchId) ?? throw new NotFoundException();
        if (match.MatchState != MatchState.Running.ToInt()) throw new BadRequestException(ErrorCodeHelper.ProcessTicket.MatchClosedOrSuspended);

        //  Channel
        var channelRepository = _inMemoryUnitOfWork.GetRepository<IChannelInMemoryRepository>();
        var channel = channelRepository.FindByRegionAndDayOfWeek(betKind.RegionId, match.KickOffTime.DayOfWeek) ?? throw new NotFoundException();

        var ticketMetadata = new TicketMetadataModel();

        //  MatchResult
        var matchResultRepository = LotteryUow.GetRepository<IMatchResultRepository>();
        var matchResult = await matchResultRepository.FindByMatchIdAndRegionIdAndChannelId(match.MatchId, betKind.RegionId, channel.Id);
        if (matchResult != null)
        {
            ticketMetadata.IsLive = matchResult.IsLive;
            if (!matchResult.EnabledProcessTicket) throw new BadRequestException(ErrorCodeHelper.ProcessTicket.ChannelIsClosed);
            if (!string.IsNullOrEmpty(matchResult.Results))
            {
                var matchResultItems = GetPrizesByRegion(betKind.RegionId, matchResult.Results);
                int? position = null;
                if (ticketMetadata.IsLive)
                {
                    position = CommonHelper.GetDefaultPositionOfPrize();
                    var firstResult = matchResultItems.SelectMany(f => f.Results).OrderBy(f => f.Position).FirstOrDefault(f => string.IsNullOrEmpty(f.Result));
                    if (firstResult != null) position = firstResult.Position;
                }
                foreach (var item in matchResultItems)
                {
                    var countResult = item.Results.Count(f => !string.IsNullOrEmpty(f.Result));
                    if (item.Results.Count == countResult) continue;

                    ticketMetadata.Position = position;
                    ticketMetadata.Prize = item.Prize;
                    ticketMetadata.EnabledProcessTicket = item.EnabledProcessTicket;
                    break;
                }
            }
        }

        return new ProcessValidationTicketModel
        {
            ClientInformation = clientInformation,
            Player = ClientContext.Player,
            BetKind = betKind,
            Channel = channel,
            Match = match,
            Metadata = ticketMetadata
        };
    }

    private List<PrizeMatchResultModel> GetPrizesByRegion(int regionId, string results)
    {
        var data = Newtonsoft.Json.JsonConvert.DeserializeObject<List<PrizeMatchResultModel>>(results).OrderBy(f => f.Prize).ToList();
        var prizeInMemoryRepository = _inMemoryUnitOfWork.GetRepository<IPrizeInMemoryRepository>();
        var prizesByRegion = prizeInMemoryRepository.FindByRegion(regionId);
        data.ForEach(f =>
        {
            var prize = prizesByRegion.Find(f1 => f1.PrizeId == f.Prize);
            if (prize == null) return;

            f.PrizeName = prize.Name;
            f.NoOfNumbers = prize.NoOfNumbers;
            for (var i = 0; i < prize.NoOfNumbers; i++)
            {
                var position = f.Prize.GetPositionOfPrize(i);
                var itemPosition = f.Results.FirstOrDefault(f1 => f1.Position == position);
                if (itemPosition == null) f.Results.Add(new PrizeMatchResultDetailModel { Position = position, Result = string.Empty });
            }
        });
        return data;
    }

    private string JoinNumbers(List<string> normalizedNumbers)
    {
        return string.Join(", ", normalizedNumbers);
    }

    public async Task LoadTicketsByMatch(long matchId, int top = -1)
    {
        var ticketRepository = LotteryUow.GetRepository<ITicketRepository>();
        var tickets = top < 0
                    ? await ticketRepository.FindQueryBy(f => !f.ParentId.HasValue && f.MatchId == matchId && f.State == TicketState.Waiting.ToInt()).ToListAsync()
                    : await ticketRepository.FindQueryBy(f => !f.ParentId.HasValue && f.MatchId == matchId && f.State == TicketState.Waiting.ToInt()).Take(top).ToListAsync();
        var ticketIds = tickets.Select(f => f.TicketId).ToList();
        var children = await ticketRepository.FindQueryBy(f => f.ParentId.HasValue && ticketIds.Contains(f.ParentId.Value)).ToListAsync();

        var ticketInMemoryRepository = _inMemoryUnitOfWork.GetRepository<ITicketInMemoryRepository>();
        foreach (var ticket in tickets)
        {
            ticketInMemoryRepository.Add(new TicketModel
            {
                TicketId = ticket.TicketId,
                CreatedAt = ticket.CreatedAt,
                Children = children.Where(f => f.ParentId.HasValue && f.ParentId.Value == ticket.TicketId).Select(f => f.TicketId).ToList()
            });
        }
    }
}