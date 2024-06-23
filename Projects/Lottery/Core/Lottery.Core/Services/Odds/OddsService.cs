using HnMicro.Framework.Exceptions;
using HnMicro.Framework.Services;
using Lottery.Core.Contexts;
using Lottery.Core.Models.Odds;
using Lottery.Core.Repositories.Agent;
using Lottery.Core.Repositories.Player;
using Lottery.Core.Services.Match;
using Lottery.Core.Services.Pubs;
using Lottery.Core.UnitOfWorks;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace Lottery.Core.Services.Odds
{
    public class OddsService : LotteryBaseService<OddsService>, IOddsService
    {
        private readonly IRunningMatchService _runningMatchService;
        private readonly IProcessOddsService _processOddsService;
        private readonly IPublishCommonService _publishCommonService;

        public OddsService(ILogger<OddsService> logger, IServiceProvider serviceProvider, IConfiguration configuration, IClockService clockService, ILotteryClientContext clientContext, ILotteryUow lotteryUow,
            IRunningMatchService runningMatchService,
            IProcessOddsService processOddsService,
            IPublishCommonService publishCommonService) : base(logger, serviceProvider, configuration, clockService, clientContext, lotteryUow)
        {
            _runningMatchService = runningMatchService;
            _processOddsService = processOddsService;
            _publishCommonService = publishCommonService;
        }

        public async Task<List<OddsModel>> GetDefaultOdds()
        {
            var agentOddRepository = LotteryUow.GetRepository<IAgentOddsRepository>();
            var defaultOdds = await agentOddRepository.FindDefaultOdds();
            return defaultOdds.Select(f => new OddsModel
            {
                Id = f.Id,
                BetKindId = f.BetKindId,
                Buy = f.Buy,
                MinBuy = f.MinBuy,
                MaxBuy = f.MaxBuy,
                MinBet = f.MinBet,
                MaxBet = f.MaxBet,
                MaxPerNumber = f.MaxPerNumber
            }).OrderBy(f => f.BetKindId).ToList();
        }

        public async Task<List<OddsModel>> GetDefaultOddsByBetKind(List<int> betKindIds)
        {
            var agentOddRepository = LotteryUow.GetRepository<IAgentOddsRepository>();
            var defaultOdds = await agentOddRepository.FindDefaultOddsByBetKind(betKindIds);
            return defaultOdds.Select(f => new OddsModel
            {
                Id = f.Id,
                BetKindId = f.BetKindId,
                Buy = f.Buy,
                MinBuy = f.MinBuy,
                MaxBuy = f.MaxBuy,
                MinBet = f.MinBet,
                MaxBet = f.MaxBet,
                MaxPerNumber = f.MaxPerNumber
            }).OrderBy(f => f.BetKindId).ToList();
        }

        public async Task<List<PlayerOddsModel>> GetMixedOddsBy(long playerId, List<int> betKindIds)
        {
            var playerOddRepository = LotteryUow.GetRepository<IPlayerOddsRepository>();
            return await playerOddRepository.FindQueryBy(f => f.PlayerId == playerId && betKindIds.Contains(f.BetKindId)).Select(f => new PlayerOddsModel
            {
                Id = f.Id,
                PlayerId = f.PlayerId,
                BetKindId = f.BetKindId,
                Buy = f.Buy
            }).ToListAsync();
        }

        public async Task<PlayerOddsModel> GetPlayerOddsBy(long playerId, int betKindId)
        {
            var playerOddRepository = LotteryUow.GetRepository<IPlayerOddsRepository>();
            return await playerOddRepository.FindQueryBy(f => f.PlayerId == playerId && f.BetKindId == betKindId).Select(f => new PlayerOddsModel
            {
                Id = f.Id,
                PlayerId = f.PlayerId,
                BetKindId = f.BetKindId,
                Buy = f.Buy
            }).FirstOrDefaultAsync();
        }

        public async Task<List<OddsModel>> GetAgentOddsBy(int betKindId, List<long> agentIds)
        {
            var agentOddRepository = LotteryUow.GetRepository<IAgentOddsRepository>();
            return await agentOddRepository.FindQueryBy(f => agentIds.Contains(f.AgentId) && f.BetKindId == betKindId).Select(f => new OddsModel
            {
                Id = f.Id,
                AgentId = f.AgentId,
                BetKindId = f.BetKindId,
                Buy = f.Buy,
                MinBuy = f.MinBuy,
                MaxBuy = f.MaxBuy,
                MinBet = f.MinBet,
                MaxBet = f.MaxBet,
                MaxPerNumber = f.MaxPerNumber
            }).ToListAsync();
        }

        public async Task UpdateAgentOdds(List<OddsModel> model, bool updateForCompany = false)
        {
            var agentOddIds = model.Select(f => f.Id).Distinct().ToList();
            var agentOddRepository = LotteryUow.GetRepository<IAgentOddsRepository>();
            var defaultOdds = new List<OddsModel>();
            var odds = await agentOddRepository.FindQueryBy(f => agentOddIds.Contains(f.Id)).ToListAsync();
            foreach (var item in odds)
            {
                var itemOdd = model.FirstOrDefault(f => f.Id == item.Id && f.BetKindId == item.BetKindId);
                if (itemOdd == null) continue;

                item.Buy = itemOdd.Buy;
                item.MinBuy = itemOdd.MinBuy;
                item.MaxBuy = itemOdd.MaxBuy;

                item.MinBet = itemOdd.MinBet;
                item.MaxBet = itemOdd.MaxBet;
                item.MaxPerNumber = itemOdd.MaxPerNumber;

                item.UpdatedAt = ClockService.GetUtcNow();
                item.UpdatedBy = ClientContext.Agent.AgentId;

                agentOddRepository.Update(item);

                if (defaultOdds.Any(f => f.Id == item.Id)) continue;
                defaultOdds.Add(itemOdd);
            }
            await LotteryUow.SaveChangesAsync();

            if (!updateForCompany) return;
            await _publishCommonService.PublishDefaultOdds(defaultOdds);
        }

        public async Task<List<OddsModel>> GetAgentOddsBy(List<int> betKindIds, List<long> agentIds)
        {
            var agentOddRepository = LotteryUow.GetRepository<IAgentOddsRepository>();
            return await agentOddRepository.FindQueryBy(f => agentIds.Contains(f.AgentId) && betKindIds.Contains(f.BetKindId)).Select(f => new OddsModel
            {
                Id = f.Id,
                AgentId = f.AgentId,
                BetKindId = f.BetKindId,
                Buy = f.Buy,
                MinBuy = f.MinBuy,
                MaxBuy = f.MaxBuy,
                MinBet = f.MinBet,
                MaxBet = f.MaxBet,
                MaxPerNumber = f.MaxPerNumber
            }).ToListAsync();
        }

        public async Task<OddsTableModel> GetOddsTableByBetKind(int betKindId)
        {
            var defaultOddsValue = (await GetDefaultOddsByBetKind(new List<int> { betKindId })).FirstOrDefault() ?? throw new BadRequestException();
            var oddsValue = new List<OddsTableDetailModel>();
            for (var i = 0; i < 100; i++) oddsValue.Add(new OddsTableDetailModel
            {
                Number = i,
                OriginValue = defaultOddsValue.Buy
            });
            var runningMatch = await _runningMatchService.GetRunningMatch();
            if (runningMatch == null)
            {
                return new OddsTableModel
                {
                    Odds = oddsValue
                };
            }

            var oddsStats = await _processOddsService.CalculateStats(runningMatch.MatchId, betKindId);
            oddsValue.ForEach(f =>
            {
                if (!oddsStats.TryGetValue(f.Number, out var oddsStatsVal)) return;

                f.Points = oddsStatsVal.Point;
                f.Payouts = oddsStatsVal.Payout;
                f.Rate = oddsStatsVal.Rate;
            });
            return new OddsTableModel
            {
                Match = runningMatch,
                Odds = oddsValue
            };
        }

        public async Task ChangeOddsValueOfOddsTable(ChangeOddsValueOfOddsTableModel model)
        {
            var runningMatch = await _runningMatchService.GetRunningMatch() ?? throw new NotFoundException();
            if (runningMatch.MatchId != model.MatchId) throw new NotFoundException();
            await _processOddsService.ChangeOddsValueOfOddsTable(model);
        }
    }
}
