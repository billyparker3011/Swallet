using HnMicro.Core.Helpers;
using HnMicro.Framework.Services;
using Lottery.Core.Helpers;
using Lottery.Core.Models.MatchResult;
using Lottery.Core.Models.Ticket;
using Lottery.Core.Options;
using Lottery.Core.Repositories.MatchResult;
using Lottery.Core.Repositories.Ticket;
using Lottery.Core.UnitOfWorks;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System.Collections.Concurrent;

namespace Lottery.Core.Services.Ticket;

public class CompletedMatchService : HnMicroBaseService<CompletedMatchService>, ICompletedMatchService, IDisposable
{
    private Timer _timer;
    private readonly ConcurrentQueue<long> _queue = new();
    private readonly CompletedMatchOption _completedMatchOption;
    private readonly List<long> _matchIds = new();
    private readonly List<int> _statesOfTicket;
    private readonly ITicketProcessor _processor;

    public CompletedMatchService(ILogger<CompletedMatchService> logger, IServiceProvider serviceProvider, IConfiguration configuration, IClockService clockService,
        ITicketProcessor processor) : base(logger, serviceProvider, configuration, clockService)
    {
        _completedMatchOption = configuration.GetSection(CompletedMatchOption.AppSettingName).Get<CompletedMatchOption>();
        _processor = processor;
        _statesOfTicket = CommonHelper.OutsTicketState();

        InitTimer();
    }

    private void InitTimer()
    {
        _timer = new Timer(CallBack, null, _completedMatchOption.IntervalInMilliseconds, Timeout.Infinite);
    }

    private void CallBack(object state)
    {
        //  Stop Timer
        _timer.Change(Timeout.Infinite, Timeout.Infinite);

        InternalCompletedMatch();

        //  Start Timer again
        _timer.Change(_completedMatchOption.IntervalInMilliseconds, Timeout.Infinite);
    }

    private void InternalCompletedMatch()
    {
        _matchIds.Clear();
        while (_queue.TryDequeue(out long matchId))
        {
            if (_matchIds.Contains(matchId)) continue;
            _matchIds.Add(matchId);
        }
        if (_matchIds.Count == 0) return;

        try
        {
            using var scope = ServiceProvider.CreateScope();
            var lotteryUow = scope.ServiceProvider.GetService<ILotteryUow>();

            var matchResultRepository = lotteryUow.GetRepository<IMatchResultRepository>();
            var matchResults = matchResultRepository.FindQueryBy(f => _matchIds.Contains(f.MatchId)).ToList();

            var ticketRepository = lotteryUow.GetRepository<ITicketRepository>();
            var tickets = ticketRepository.FindQueryBy(f => _matchIds.Contains(f.MatchId) && _statesOfTicket.Contains(f.State)).ToList();

            var rootTickets = tickets.Where(f => !f.ParentId.HasValue).ToList();
            foreach (var item in rootTickets)
            {
                var matchResult = matchResults.FirstOrDefault(f => f.MatchId == item.MatchId && f.RegionId == item.RegionId && f.ChannelId == item.ChannelId);
                if (matchResult == null || string.IsNullOrEmpty(matchResult.Results)) continue;
                var result = Newtonsoft.Json.JsonConvert.DeserializeObject<List<PrizeMatchResultModel>>(matchResult.Results);

                var children = tickets.Where(f => f.ParentId == item.TicketId).ToList();
                var resultTicket = _processor.CompletedTicket(item.BetKindId, new CompletedTicketModel
                {
                    TicketId = item.TicketId,
                    ChoosenNumbers = item.ChoosenNumbers,
                    Stake = item.Stake,

                    Prize = item.Prize,
                    Position = item.Position,

                    PlayerOdds = item.PlayerOdds,
                    PlayerPayout = item.PlayerPayout,

                    AgentOdds = item.AgentOdds,
                    AgentPayout = item.AgentPayout,
                    AgentPt = item.AgentPt,

                    MasterOdds = item.MasterOdds,
                    MasterPayout = item.MasterPayout,
                    MasterPt = item.MasterPt,

                    SupermasterOdds = item.SupermasterOdds,
                    SupermasterPayout = item.SupermasterPayout,
                    SupermasterPt = item.SupermasterPt,

                    CompanyOdds = item.CompanyOdds,
                    CompanyPayout = item.CompanyPayout,

                    RewardRate = item.RewardRate,
                    Children = children.Select(f => new CompletedChildrenTicketModel
                    {
                        TicketId = f.TicketId,
                        ParentId = f.ParentId,
                        ChoosenNumbers = f.ChoosenNumbers,
                        Stake = f.Stake,

                        PlayerOdds = f.PlayerOdds,
                        PlayerPayout = f.PlayerPayout,

                        AgentOdds = item.AgentOdds,
                        AgentPayout = item.AgentPayout,
                        AgentPt = item.AgentPt,

                        MasterOdds = item.MasterOdds,
                        MasterPayout = item.MasterPayout,
                        MasterPt = item.MasterPt,

                        SupermasterOdds = item.SupermasterOdds,
                        SupermasterPayout = item.SupermasterPayout,
                        SupermasterPt = item.SupermasterPt,

                        CompanyOdds = item.CompanyOdds,
                        CompanyPayout = item.CompanyPayout
                    }).ToList()
                }, result);
                if (resultTicket == null) continue;

                item.Times = resultTicket.Times;
                item.MixedTimes = resultTicket.MixedTimes;

                item.State = resultTicket.State.ToInt();
                item.PlayerWinLoss = resultTicket.PlayerWinLose;
                item.DraftPlayerWinLoss = resultTicket.DraftPlayerWinLose;

                item.AgentWinLoss = resultTicket.AgentWinLose;
                item.AgentCommission = resultTicket.AgentCommission;

                item.MasterPayout = resultTicket.MasterWinLose;
                item.MasterCommission = resultTicket.MasterCommission;

                item.SupermasterPayout = resultTicket.SupermasterWinLose;
                item.SupermasterCommission = resultTicket.SupermasterCommission;

                item.CompanyWinLoss = resultTicket.CompanyWinLose;
                item.UpdatedAt = ClockService.GetUtcNow();

                ticketRepository.Update(item);

                foreach (var subTicket in resultTicket.Children)
                {
                    var child = children.FirstOrDefault(f => f.TicketId == subTicket.TicketId);
                    if (child == null) continue;

                    child.Times = subTicket.Times;
                    child.MixedTimes = subTicket.MixedTimes;

                    child.State = subTicket.State.ToInt();

                    child.PlayerWinLoss = subTicket.PlayerWinLose;

                    child.AgentWinLoss = subTicket.AgentWinLose;
                    child.AgentCommission = subTicket.AgentCommission;

                    child.MasterWinLoss = subTicket.MasterWinLose;
                    child.MasterCommission = subTicket.MasterCommission;

                    child.SupermasterWinLoss = subTicket.SupermasterWinLose;
                    child.SupermasterCommission = subTicket.SupermasterCommission;

                    child.CompanyWinLoss = subTicket.CompanyWinLose;
                    child.UpdatedAt = item.UpdatedAt;

                    ticketRepository.Update(child);
                }
            }

            lotteryUow.SaveChanges();
        }
        catch (Exception ex)
        {
            Logger.LogError(ex, ex.Message, ex.StackTrace);
        }
    }

    public void Enqueue(long matchId)
    {
        _queue.Enqueue(matchId);
    }

    public void Dispose()
    {
        _timer?.Dispose();
    }
}