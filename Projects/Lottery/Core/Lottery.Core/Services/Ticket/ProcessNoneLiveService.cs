using HnMicro.Core.Helpers;
using HnMicro.Framework.Enums;
using HnMicro.Framework.Exceptions;
using HnMicro.Framework.Services;
using Lottery.Core.Contexts;
using Lottery.Core.Enums;
using Lottery.Core.Helpers;
using Lottery.Core.Models.PositionTakings;
using Lottery.Core.Models.Ticket.Process;
using Lottery.Core.Repositories.Ticket;
using Lottery.Core.Services.Agent;
using Lottery.Core.Services.Caching.Player;
using Lottery.Core.Services.Odds;
using Lottery.Core.Services.Player;
using Lottery.Core.UnitOfWorks;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace Lottery.Core.Services.Ticket;

public class ProcessNoneLiveService : LotteryBaseService<ProcessNoneLiveService>, IProcessNoneLiveService
{
    private readonly ITicketProcessor _ticketProcessor;
    private readonly IAgentPositionTakingService _agentPositionTakingService;
    private readonly IPlayerSettingService _playerSettingService;
    private readonly IProcessTicketService _processTicketService;
    private readonly IProcessOddsService _processOddsService;

    public ProcessNoneLiveService(ILogger<ProcessNoneLiveService> logger, IServiceProvider serviceProvider, IConfiguration configuration, IClockService clockService, ILotteryClientContext clientContext, ILotteryUow lotteryUow,
        ITicketProcessor ticketProcessor,
        IAgentPositionTakingService agentPositionTakingService,
        IPlayerSettingService playerSettingService,
        IProcessTicketService processTicketService,
        IProcessOddsService processOddsService) : base(logger, serviceProvider, configuration, clockService, clientContext, lotteryUow)
    {
        _ticketProcessor = ticketProcessor;
        _agentPositionTakingService = agentPositionTakingService;
        _playerSettingService = playerSettingService;
        _processTicketService = processTicketService;
        _processOddsService = processOddsService;
    }

    public async Task<(Data.Entities.Ticket, List<Data.Entities.Ticket>)> Process(ProcessTicketModel model, ProcessValidationTicketModel processValidation)
    {
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
            KickOffTime = processValidation.Match.KickoffTime,
            RegionId = processValidation.BetKind.RegionId,
            ChannelId = processValidation.Channel.Id,
            ChoosenNumbers = string.Empty,

            RewardRate = processValidation.BetKind.Award,
            Stake = 0m,
            PlayerOdds = 0m,
            PlayerPayout = 0m,
            PlayerWinLoss = 0m,
            DraftPlayerWinLoss = 0m,

            AgentOdds = 0m,
            AgentPayout = 0m,
            AgentWinLoss = 0m,
            AgentCommission = 0m,
            AgentPt = 0m,
            DraftAgentWinLoss = 0m,
            DraftAgentCommission = 0m,

            MasterOdds = 0m,
            MasterPayout = 0m,
            MasterWinLoss = 0m,
            MasterCommission = 0m,
            MasterPt = 0m,
            DraftMasterWinLoss = 0m,
            DraftMasterCommission = 0m,

            SupermasterOdds = 0m,
            SupermasterPayout = 0m,
            SupermasterWinLoss = 0m,
            SupermasterCommission = 0m,
            SupermasterPt = 0m,
            DraftSupermasterWinLoss = 0m,
            DraftSupermasterCommission = 0m,

            CompanyOdds = 0m,
            CompanyPayout = 0m,
            CompanyWinLoss = 0m,
            DraftCompanyWinLoss = 0m,

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
                    DraftPlayerWinLoss = ticket.DraftPlayerWinLoss,

                    AgentOdds = aOddsValue,
                    AgentPayout = agentPayout,
                    AgentWinLoss = ticket.AgentWinLoss,
                    AgentPt = ticket.AgentPt,
                    DraftAgentWinLoss = ticket.DraftAgentWinLoss,

                    MasterOdds = mOddsValue,
                    MasterPayout = masterPayout,
                    MasterWinLoss = ticket.MasterWinLoss,
                    MasterPt = ticket.MasterPt,
                    DraftMasterWinLoss = ticket.DraftMasterWinLoss,

                    SupermasterOdds = sOddsValue,
                    SupermasterPayout = supermasterPayout,
                    SupermasterWinLoss = ticket.SupermasterWinLoss,
                    SupermasterPt = ticket.SupermasterPt,
                    DraftSupermasterWinLoss = ticket.DraftSupermasterWinLoss,

                    CompanyOdds = cOddsValue,
                    CompanyPayout = companyPayout,
                    CompanyWinLoss = ticket.CompanyWinLoss,
                    DraftCompanyWinLoss = ticket.DraftCompanyWinLoss,

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
        await _processTicketService.BuildPointsByMatchAndNumbersCache(processValidation.Player.PlayerId, processValidation.Match.MatchId, outs.PointsByMatchAndNumbers, pointByNumbers);
        if (enableStats) await _processTicketService.BuildStatsByMatchBetKindAndNumbers(processValidation.Match.MatchId, processValidation.BetKind.Id, pointByNumbers, payoutByNumbers);

        return (ticket, childTickets);
    }
}
