using HnMicro.Core.Helpers;
using HnMicro.Framework.Services;
using Lottery.Core.Contexts;
using Lottery.Core.Enums;
using Lottery.Core.Models.Statement;
using Lottery.Core.Repositories.Match;
using Lottery.Core.Repositories.Ticket;
using Lottery.Core.UnitOfWorks;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace Lottery.Core.Services.Statement
{
    public class StatementService : LotteryBaseService<StatementService>, IStatementService
    {
        public StatementService(ILogger<StatementService> logger, IServiceProvider serviceProvider, IConfiguration configuration, IClockService clockService, ILotteryClientContext clientContext, ILotteryUow lotteryUow) : base(logger, serviceProvider, configuration, clockService, clientContext, lotteryUow)
        {
        }

        public async Task<List<StatementModel>> GetMyStatement()
        {
            var matchRepository = LotteryUow.GetRepository<IMatchRepository>();
            var topMatch = await matchRepository.FindQueryBy(f => true).OrderByDescending(f => f.MatchId).Take(15).ToListAsync();

            var matchIds = topMatch.Select(f => f.MatchId).ToList();
            var ticketRepository = LotteryUow.GetRepository<ITicketRepository>();
            var tickets = await ticketRepository.FindQueryBy(f => !f.ParentId.HasValue && matchIds.Contains(f.MatchId) && f.PlayerId == ClientContext.Player.PlayerId)
                .GroupBy(f => f.MatchId)
                .Select(f => new
                {
                    MatchId = f.Key,
                    TotalPoints = f.Sum(f1 => f1.Stake),
                    TotalPayout = f.Sum(f1 => f1.PlayerPayout),
                    TotalWinlose = f.Sum(f1 => f1.PlayerWinLoss)
                }).ToListAsync();

            var statements = new List<StatementModel>();
            foreach (var match in topMatch)
            {
                var statementByMatch = tickets.FirstOrDefault(f => f.MatchId == match.MatchId);
                statements.Add(new StatementModel
                {
                    MatchId = match.MatchId,
                    MatchCode = match.MatchCode,
                    Kickofftime = match.KickOffTime,
                    TotalPoint = statementByMatch != null ? statementByMatch.TotalPoints : 0m,
                    TotalPayout = statementByMatch != null ? statementByMatch.TotalPayout : 0m,
                    TotalWinlose = match.MatchState == MatchState.Completed.ToInt() && statementByMatch != null ? statementByMatch.TotalWinlose : null
                });
            }
            return statements;
        }
    }
}
