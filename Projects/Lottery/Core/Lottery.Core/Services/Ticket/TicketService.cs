using HnMicro.Core.Helpers;
using HnMicro.Framework.Exceptions;
using HnMicro.Framework.Services;
using HnMicro.Modules.InMemory.UnitOfWorks;
using Lottery.Core.Contexts;
using Lottery.Core.Enums;
using Lottery.Core.Helpers;
using Lottery.Core.InMemory.BetKind;
using Lottery.Core.InMemory.Channel;
using Lottery.Core.InMemory.Ticket;
using Lottery.Core.Models.MatchResult;
using Lottery.Core.Models.Ticket;
using Lottery.Core.Models.Ticket.Process;
using Lottery.Core.Repositories.Ticket;
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
    private readonly IPlayerService _playerService;
    private readonly IProcessNoneLiveService _processNoneLiveService;
    private readonly IProcessLiveService _processLiveService;
    private readonly IProcessMixedService _processMixedService;
    private readonly INumberService _numberService;

    public TicketService(ILogger<TicketService> logger, IServiceProvider serviceProvider, IConfiguration configuration, IClockService clockService, ILotteryClientContext clientContext, ILotteryUow lotteryUow,
        IInMemoryUnitOfWork inMemoryUnitOfWork,
        IRunningMatchService runningMatchService,
        ITicketProcessor ticketProcessor,
        IPlayerService playerService,
        IProcessNoneLiveService processNoneLiveService,
        IProcessLiveService processLiveService,
        IProcessMixedService processMixedService,
        INumberService numberService) : base(logger, serviceProvider, configuration, clockService, clientContext, lotteryUow)
    {
        _inMemoryUnitOfWork = inMemoryUnitOfWork;
        _runningMatchService = runningMatchService;
        _ticketProcessor = ticketProcessor;
        _playerService = playerService;
        _processNoneLiveService = processNoneLiveService;
        _processLiveService = processLiveService;
        _processMixedService = processMixedService;
        _numberService = numberService;
    }

    public async Task Process(ProcessTicketModel model)
    {
        var processValidation = await InternalProcess(model.BetKindId, model.Numbers.Select(f => f.Number).ToList());

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

        var errCode = _ticketProcessor.ValidMixed(model, processValidation.Metadata);
        if (errCode < 0) throw new BadRequestException(errCode);

        var tickets = await _processMixedService.Process(model, processValidation);
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

        //  Checking Numbers was suspended
        var suspendedNumbers = await _numberService.GetSuspendedNumbersByMatchAndBetKind(match.MatchId, betKindId);
        if (suspendedNumbers.Count > 0 && numbers.Any(suspendedNumbers.Contains))
        {
            var normalizeSuspendedNumbers = string.Join(", ", suspendedNumbers.Select(f => f.NormalizeNumber(noOfNumbers))).Trim();
            throw new BadRequestException(ErrorCodeHelper.ProcessTicket.NumbersWasSuspended, normalizeSuspendedNumbers);
        }

        var ticketMetadata = new TicketMetadataModel();
        Models.Channel.ChannelModel channel = null;
        var mixedChannels = betKindId.IsMixedChannels();

        if (mixedChannels)
        {
            ticketMetadata.IsLive = matchResultDetail.Any(f => f.IsLive);
            ticketMetadata.AllowProcessTicket = matchResultDetail.Count(f => f.EnabledProcessTicket) == matchResultDetail.Count;
        }
        else
        {
            //  Channel
            var channelRepository = _inMemoryUnitOfWork.GetRepository<IChannelInMemoryRepository>();
            channel = channelRepository.FindByRegionAndDayOfWeek(betKind.RegionId, match.KickoffTime.DayOfWeek) ?? throw new NotFoundException();

            //  MatchResult
            var resultByChannel = matchResultDetail.FirstOrDefault(f => f.ChannelId == channel.Id) ?? throw new NotFoundException();
            if (!resultByChannel.EnabledProcessTicket) throw new BadRequestException(ErrorCodeHelper.ProcessTicket.ChannelIsClosed);

            ticketMetadata.IsLive = resultByChannel.IsLive;
            if (resultByChannel.IsLive)
            {
                var results = resultByChannel.Prize.SelectMany(f => f.Results).OrderBy(f => f.Position).ToList();
                (var currentPrize, var currentPosition) = _runningMatchService.GetCurrentPrize(betKind.RegionId, resultByChannel.Prize);
                if (currentPrize == null || currentPosition == null) throw new BadRequestException(ErrorCodeHelper.ProcessTicket.PrizeOrPostionIsInvalid);

                ticketMetadata.Prize = currentPrize.Prize;
                ticketMetadata.Position = currentPosition.Position;
                ticketMetadata.AllowProcessTicket = currentPosition.AllowProcessTicket;
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

    public async Task ProcessV2(ProcessTicketV2Model model)
    {
        var processValidation = await InternalProcessV2(model);
        foreach (var item in model.Details)
        {
            var errCode = _ticketProcessor.ValidV2(model, item.BetKindId, processValidation.Details);
            if (errCode < 0) throw new BadRequestException(errCode);

            if (item.BetKindId == Enums.BetKind.FirstNorthern_Northern_LoLive.ToInt() ||
                item.BetKindId == Enums.BetKind.Central_2D18LoLive.ToInt() ||
                item.BetKindId == Enums.BetKind.Southern_2D18LoLive.ToInt())
            {
                (var ticket1, var childTickets1) = await _processLiveService.ProcessV2(item, processValidation);
                AddToAcceptedScanService(ticket1, childTickets1);
                continue;
            }

            (var ticket2, var childTickets2) = await _processNoneLiveService.ProcessV2(item, processValidation);
            AddToAcceptedScanService(ticket2, childTickets2);
        }
    }

    private async Task<ProcessValidationTicketV2Model> InternalProcessV2(ProcessTicketV2Model model)
    {
        //  Check auth
        var clientInformation = ClientContext.GetClientInformation();
        var player = ClientContext.Player;
        if (clientInformation == null || player == null || player.PlayerId <= 0L) throw new UnauthorizedException();

        //  Player information
        //  TODO Convert to cache
        var playerInfo = await _playerService.GetPlayer(player.PlayerId) ?? throw new UnauthorizedException();
        if (playerInfo.Lock) throw new UnauthorizedException();
        if (playerInfo.State.IsSuspended() || (playerInfo.ParentState != null && playerInfo.ParentState.Value.IsSuspended())) throw new BadRequestException(ErrorCodeHelper.ProcessTicket.PlayerIsSuspended);
        if (playerInfo.State.IsClosed() || (playerInfo.ParentState != null && playerInfo.ParentState.Value.IsClosed())) throw new BadRequestException(ErrorCodeHelper.ProcessTicket.PlayerIsClosed);

        //  Check MatchId
        var match = await _runningMatchService.GetRunningMatch() ?? throw new NotFoundException();
        if (match.MatchId != model.MatchId) throw new BadRequestException(ErrorCodeHelper.ProcessTicket.CannotFindMatch);
        if (match.State != MatchState.Running.ToInt()) throw new BadRequestException(ErrorCodeHelper.ProcessTicket.MatchClosedOrSuspended);

        //  BetKind
        var betKindRepository = _inMemoryUnitOfWork.GetRepository<IBetKindInMemoryRepository>();

        //  Channel
        var channelRepository = _inMemoryUnitOfWork.GetRepository<IChannelInMemoryRepository>();

        var rs = new ProcessValidationTicketV2Model
        {
            ClientInformation = clientInformation,
            Player = ClientContext.Player,
            Match = match,
            Details = new List<ProcessValidationTicketDetailV2Model>()
        };

        foreach (var item in model.Details)
        {
            var noOfNumbers = item.BetKindId.GetNoOfNumbers();
            var numbers = item.Numbers.Select(f => f.Number).ToList();
            if (numbers.Any(f => f > noOfNumbers)) throw new BadRequestException(ErrorCodeHelper.ProcessTicket.NumberIsLessThan, noOfNumbers.ToString());

            //  Check BetKindId
            var betKind = betKindRepository.FindById(item.BetKindId) ?? throw new NotFoundException();
            if (!betKind.Enabled) throw new BadRequestException(ErrorCodeHelper.ProcessTicket.TheBetKindDoesNotAllowProcessing);
            if (!match.MatchResult.TryGetValue(betKind.RegionId, out List<ResultByRegionModel> matchResultDetail)) throw new NotFoundException();

            //  Checking Numbers was suspended
            var suspendedNumbers = await _numberService.GetSuspendedNumbersByMatchAndBetKind(match.MatchId, item.BetKindId);
            if (suspendedNumbers.Count > 0 && numbers.Any(suspendedNumbers.Contains))
            {
                var normalizeSuspendedNumbers = string.Join(", ", suspendedNumbers.Select(f => f.NormalizeNumber(noOfNumbers))).Trim();
                throw new BadRequestException(ErrorCodeHelper.ProcessTicket.NumbersWasSuspended, normalizeSuspendedNumbers);
            }

            var resultByChannel = matchResultDetail.FirstOrDefault(f => f.ChannelId == item.ChannelId) ?? throw new NotFoundException();
            if (!resultByChannel.EnabledProcessTicket) throw new BadRequestException(ErrorCodeHelper.ProcessTicket.ChannelIsClosed);

            var channel = channelRepository.FindById(item.ChannelId) ?? throw new NotFoundException();

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

            rs.Details.Add(new ProcessValidationTicketDetailV2Model
            {
                BetKind = betKind,
                Channel = channel,
                Metadata = ticketMetadata
            });
        }
        return rs;
    }

    public async Task ProcessMixedV2(ProcessMixedTicketV2Model model)
    {
        var processValidation = await InternalProcessMixedV2(model.BetKindId, model.Numbers, model.ChannelIds);
        foreach (var item in processValidation.Details)
        {
            var errCode = _ticketProcessor.ValidMixedV2(model, item.Metadata);
            if (errCode < 0) throw new BadRequestException(errCode);

            var tickets = await _processMixedService.ProcessV2(model, processValidation, item);
            AddToAcceptedScanMixedService(tickets);
        }
    }

    private async Task<ProcessValidationTicketV2Model> InternalProcessMixedV2(int betKindId, List<int> numbers, List<int> channelIds)
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
        if (betKind.CorrelationBetKindIds.Count > 0)
        {
            var childBetKind = betKindRepository.FindBy(f => betKind.CorrelationBetKindIds.Contains(f.Id) && !f.Enabled).ToList();
            if (childBetKind.Count > 0) throw new BadRequestException(ErrorCodeHelper.ProcessTicket.TheBetKindDoesNotAllowProcessing);
        }

        //  Check MatchId
        var match = await _runningMatchService.GetRunningMatch() ?? throw new NotFoundException();
        if (match.State != MatchState.Running.ToInt()) throw new BadRequestException(ErrorCodeHelper.ProcessTicket.MatchClosedOrSuspended);
        if (!match.MatchResult.TryGetValue(betKind.RegionId, out List<ResultByRegionModel> matchResultDetail)) throw new NotFoundException();

        var rs = new ProcessValidationTicketV2Model
        {
            ClientInformation = clientInformation,
            Player = ClientContext.Player,
            Match = match,
            Details = new List<ProcessValidationTicketDetailV2Model>()
        };

        //  Channel
        var channelRepository = _inMemoryUnitOfWork.GetRepository<IChannelInMemoryRepository>();

        foreach (var channelId in channelIds)
        {
            var channel = channelRepository.FindById(channelId) ?? throw new NotFoundException();
            var resultByChannel = matchResultDetail.FirstOrDefault(f => f.ChannelId == channelId) ?? throw new NotFoundException();
            if (!resultByChannel.EnabledProcessTicket) throw new BadRequestException(ErrorCodeHelper.ProcessTicket.ChannelIsClosed);

            var metadata = new TicketMetadataModel
            {
                IsLive = resultByChannel.IsLive
            };
            if (resultByChannel.IsLive)
            {
                var results = resultByChannel.Prize.SelectMany(f => f.Results).OrderBy(f => f.Position).ToList();
                (var currentPrize, var currentPosition) = _runningMatchService.GetCurrentPrize(betKind.RegionId, resultByChannel.Prize);
                if (currentPrize == null || currentPosition == null) throw new BadRequestException(ErrorCodeHelper.ProcessTicket.PrizeOrPostionIsInvalid);

                metadata.Prize = currentPrize.Prize;
                metadata.Position = currentPosition.Position;
                metadata.AllowProcessTicket = currentPosition.AllowProcessTicket;
            }

            rs.Details.Add(new ProcessValidationTicketDetailV2Model
            {
                BetKind = betKind,
                Channel = channel,
                Metadata = metadata
            });
        }

        //  Checking Numbers was suspended
        var suspendedNumbers = await _numberService.GetSuspendedNumbersByMatchAndBetKind(match.MatchId, betKindId);
        if (suspendedNumbers.Count > 0 && numbers.Any(suspendedNumbers.Contains))
        {
            var normalizeSuspendedNumbers = string.Join(", ", suspendedNumbers.Select(f => f.NormalizeNumber(noOfNumbers))).Trim();
            throw new BadRequestException(ErrorCodeHelper.ProcessTicket.NumbersWasSuspended, normalizeSuspendedNumbers);
        }

        return rs;
    }
}