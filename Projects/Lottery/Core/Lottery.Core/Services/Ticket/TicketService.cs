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
using Lottery.Core.InMemory.Ticket;
using Lottery.Core.Models.BetKind;
using Lottery.Core.Models.MatchResult;
using Lottery.Core.Models.PositionTakings;
using Lottery.Core.Models.Setting;
using Lottery.Core.Models.Ticket;
using Lottery.Core.Models.Ticket.Process;
using Lottery.Core.Repositories.Ticket;
using Lottery.Core.Services.Agent;
using Lottery.Core.Services.Caching.Player;
using Lottery.Core.Services.Match;
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
    private readonly IRunningMatchService _runningMatchService;
    private readonly ITicketProcessor _ticketProcessor;
    private readonly IAgentPositionTakingService _agentPositionTakingService;
    private readonly IPlayerService _playerService;
    private readonly IPlayerSettingService _playerSettingService;
    private readonly IProcessTicketService _processTicketService;
    private readonly IProcessOddsService _processOddsService;
    private readonly IProcessNoneLiveService _processNoneLiveService;
    private readonly IProcessLiveService _processLiveService;
    private readonly INumberService _numberService;

    public TicketService(ILogger<TicketService> logger, IServiceProvider serviceProvider, IConfiguration configuration, IClockService clockService, ILotteryClientContext clientContext, ILotteryUow lotteryUow,
        IInMemoryUnitOfWork inMemoryUnitOfWork,
        IRunningMatchService runningMatchService,
        ITicketProcessor ticketProcessor,
        IAgentPositionTakingService agentPositionTakingService,
        IPlayerService playerService,
        IPlayerSettingService playerSettingService,
        IProcessTicketService processTicketService,
        IProcessOddsService processOddsService,
        IProcessNoneLiveService processNoneLiveService,
        IProcessLiveService processLiveService,
        INumberService numberService) : base(logger, serviceProvider, configuration, clockService, clientContext, lotteryUow)
    {
        _inMemoryUnitOfWork = inMemoryUnitOfWork;
        _runningMatchService = runningMatchService;
        _ticketProcessor = ticketProcessor;
        _agentPositionTakingService = agentPositionTakingService;
        _playerService = playerService;
        _playerSettingService = playerSettingService;
        _processTicketService = processTicketService;
        _processOddsService = processOddsService;
        _processNoneLiveService = processNoneLiveService;
        _processLiveService = processLiveService;
        _numberService = numberService;
    }

    public async Task Process(ProcessTicketModel model)
    {
        var processValidation = await InternalProcess(model.BetKindId, model.Numbers.Select(f => f.Number).ToList());

        //  Validation BetKind
        var errCode = _ticketProcessor.Valid(model, processValidation.Metadata);
        if (errCode < 0) throw new BadRequestException(errCode);

        if (model.BetKindId == Enums.BetKind.FirstNorthern_Northern_LoLive.ToInt())
        {
            (var ticket1, var childTickets1) = await _processLiveService.Process(model, processValidation);
            AddToAcceptedScanService(ticket1, childTickets1);
            return;
        }

        (var ticket2, var childTickets2) = await _processNoneLiveService.Process(model, processValidation);
        AddToAcceptedScanService(ticket2, childTickets2);
    }

    private void AddToAcceptedScanService(Data.Entities.Ticket ticket, List<Data.Entities.Ticket> childTickets)
    {
        var ticketInMemoryRepository = _inMemoryUnitOfWork.GetRepository<ITicketInMemoryRepository>();
        ticketInMemoryRepository.Add(new TicketModel
        {
            TicketId = ticket.TicketId,
            IsLive = ticket.IsLive,
            CreatedAt = ticket.CreatedAt,
            Children = childTickets.Select(f => f.TicketId).ToList()
        });
    }

    public async Task ProcessMixed(ProcessMixedTicketModel model)
    {
        var processValidation = await InternalProcess(model.BetKindId, model.Numbers);

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
            IsLive = f.IsLive,
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
            DraftPlayerWinLoss = 0m,
            //  Agent
            AgentOdds = 0m,
            AgentPayout = 0m,
            AgentWinLoss = 0m,
            DraftAgentWinLoss = 0m,
            //  Master
            MasterOdds = 0m,
            MasterPayout = 0m,
            MasterWinLoss = 0m,
            DraftMasterWinLoss = 0m,
            //  Supermaster
            SupermasterOdds = 0m,
            SupermasterPayout = 0m,
            SupermasterWinLoss = 0m,
            DraftSupermasterWinLoss = 0m,
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
            KickOffTime = processValidation.Match.KickoffTime,
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
            DraftPlayerWinLoss = 0m,
            //  Agent
            AgentOdds = 0m,
            AgentPayout = 0m,
            AgentWinLoss = 0m,
            DraftAgentWinLoss = 0m,
            //  Master
            MasterOdds = 0m,
            MasterPayout = 0m,
            MasterWinLoss = 0m,
            DraftMasterWinLoss = 0m,
            //  Supermaster
            SupermasterOdds = 0m,
            SupermasterPayout = 0m,
            SupermasterWinLoss = 0m,
            DraftSupermasterWinLoss = 0m,
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
            KickOffTime = processValidation.Match.KickoffTime,
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
            DraftPlayerWinLoss = 0m,
            //  Agent
            AgentOdds = 0m,
            AgentPayout = 0m,
            AgentWinLoss = 0m,
            AgentPt = 0m,
            DraftAgentWinLoss = 0m,
            //  Master
            MasterOdds = 0m,
            MasterPayout = 0m,
            MasterWinLoss = 0m,
            DraftMasterWinLoss = 0m,
            //  Supermaster
            SupermasterOdds = 0m,
            SupermasterPayout = 0m,
            SupermasterWinLoss = 0m,
            DraftSupermasterWinLoss = 0m,
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

    private async Task<ProcessValidationTicketModel> InternalProcess(int betKindId, List<int> numbers)
    {
        //  Check auth
        var clientInformation = ClientContext.GetClientInformation();
        var player = ClientContext.Player;
        if (clientInformation == null || player == null || player.PlayerId <= 0L) throw new UnauthorizedException();

        var noOfNumbers = betKindId.GetNoOfNumbers();
        if (numbers.Any(f => f > noOfNumbers)) throw new BadRequestException(ErrorCodeHelper.ProcessTicket.NumberIsLessThan, noOfNumbers.ToString());

        //  Player information
        //  TODO Convert to cache
        var playerInfo = await _playerService.GetPlayer(player.PlayerId) ?? throw new UnauthorizedException();
        if (playerInfo.Lock) throw new UnauthorizedException();
        if (playerInfo.State.IsSuspended() || (playerInfo.ParentState != null && playerInfo.ParentState.Value.IsSuspended())) throw new BadRequestException(ErrorCodeHelper.ProcessTicket.PlayerIsSuspended);
        if (playerInfo.State.IsClosed() || (playerInfo.ParentState != null && playerInfo.ParentState.Value.IsClosed())) throw new BadRequestException(ErrorCodeHelper.ProcessTicket.PlayerIsClosed);

        //  Check BetKindId
        var betKindRepository = _inMemoryUnitOfWork.GetRepository<IBetKindInMemoryRepository>();
        var betKind = betKindRepository.FindById(betKindId) ?? throw new NotFoundException();
        if (!betKind.Enabled) throw new BadRequestException(ErrorCodeHelper.ProcessTicket.TheBetKindDoesNotAllowProcessing);

        //  Check MatchId
        var match = await _runningMatchService.GetRunningMatch() ?? throw new NotFoundException();
        if (match.State != MatchState.Running.ToInt()) throw new BadRequestException(ErrorCodeHelper.ProcessTicket.MatchClosedOrSuspended);
        if (!match.MatchResult.TryGetValue(betKind.RegionId, out List<ResultByRegionModel> matchResultDetail)) throw new NotFoundException();

        //  Channel
        var channelRepository = _inMemoryUnitOfWork.GetRepository<IChannelInMemoryRepository>();
        var channel = channelRepository.FindByRegionAndDayOfWeek(betKind.RegionId, match.KickoffTime.DayOfWeek) ?? throw new NotFoundException();

        //  Checking Numbers was suspended
        var suspendedNumbers = await _numberService.GetSuspendedNumbersByMatchAndBetKind(match.MatchId, betKindId);
        if (suspendedNumbers.Count > 0 && numbers.Any(suspendedNumbers.Contains))
        {
            var normalizeSuspendedNumbers = string.Join(", ", suspendedNumbers.Select(f => f.NormalizeNumber(noOfNumbers))).Trim();
            throw new BadRequestException(ErrorCodeHelper.ProcessTicket.NumbersWasSuspended, normalizeSuspendedNumbers);
        }

        //  MatchResult
        var resultByChannel = matchResultDetail.FirstOrDefault(f => f.ChannelId == channel.Id) ?? throw new NotFoundException();
        if (!resultByChannel.EnabledProcessTicket) throw new BadRequestException(ErrorCodeHelper.ProcessTicket.ChannelIsClosed);

        var ticketMetadata = new TicketMetadataModel
        {
            IsLive = resultByChannel.IsLive
        };
        if (ticketMetadata.IsLive)
        {
            var results = resultByChannel.Prize.SelectMany(f => f.Results).OrderBy(f => f.Position).ToList();
            (var currentPrize, var currentPosition) = _runningMatchService.GetCurrentPrize(betKind.RegionId, resultByChannel.Prize);
            if (currentPrize == null || currentPosition == null) throw new BadRequestException(ErrorCodeHelper.ProcessTicket.PrizeOrPostionIsInvalid);

            ticketMetadata.Prize = currentPrize.Prize;
            ticketMetadata.Position = currentPosition.Position;
            ticketMetadata.AllowProcessTicket = currentPosition.AllowProcessTicket;
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
                IsLive = ticket.IsLive,
                CreatedAt = ticket.CreatedAt,
                Children = children.Where(f => f.ParentId.HasValue && f.ParentId.Value == ticket.TicketId).Select(f => f.TicketId).ToList()
            });
        }
    }
}