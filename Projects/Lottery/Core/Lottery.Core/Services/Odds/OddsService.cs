using HnMicro.Core.Helpers;
using HnMicro.Framework.Exceptions;
using HnMicro.Framework.Services;
using HnMicro.Modules.InMemory.UnitOfWorks;
using Lottery.Core.Contexts;
using Lottery.Core.Enums;
using Lottery.Core.Helpers;
using Lottery.Core.InMemory.BetKind;
using Lottery.Core.Models.Match;
using Lottery.Core.Models.MatchResult;
using Lottery.Core.Models.Odds;
using Lottery.Core.Repositories.Agent;
using Lottery.Core.Repositories.Player;
using Lottery.Core.Services.Match;
using Lottery.Core.Services.Others;
using Lottery.Core.Services.Pubs;
using Lottery.Core.UnitOfWorks;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace Lottery.Core.Services.Odds
{
    public class OddsService : LotteryBaseService<OddsService>, IOddsService
    {
        private readonly IInMemoryUnitOfWork _inMemoryUnitOfWork;
        private readonly IRunningMatchService _runningMatchService;
        private readonly IProcessOddsService _processOddsService;
        private readonly INormalizeValueService _normalizeValueService;
        private readonly IPublishCommonService _publishCommonService;

        public OddsService(ILogger<OddsService> logger, IServiceProvider serviceProvider, IConfiguration configuration, IClockService clockService, ILotteryClientContext clientContext, ILotteryUow lotteryUow,
            IInMemoryUnitOfWork inMemoryUnitOfWork,
            IRunningMatchService runningMatchService,
            IProcessOddsService processOddsService,
            INormalizeValueService normalizeValueService,
            IPublishCommonService publishCommonService) : base(logger, serviceProvider, configuration, clockService, clientContext, lotteryUow)
        {
            _inMemoryUnitOfWork = inMemoryUnitOfWork;
            _runningMatchService = runningMatchService;
            _processOddsService = processOddsService;
            _normalizeValueService = normalizeValueService;
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

        public async Task<List<PlayerOddsModel>> GetOddsByListBetKind(long playerId, List<int> betKindIds)
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

        public async Task<List<PlayerOddsModel>> GetMixedOddsBy(List<long> playerIds, List<int> betKindIds)
        {
            var playerOddRepository = LotteryUow.GetRepository<IPlayerOddsRepository>();
            return await playerOddRepository.FindQueryBy(f => playerIds.Contains(f.PlayerId) && betKindIds.Contains(f.BetKindId)).Select(f => new PlayerOddsModel
            {
                Id = f.Id,
                PlayerId = f.PlayerId,
                BetKindId = f.BetKindId,
                Buy = f.Buy
            }).ToListAsync();
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
            var noOfNumbers = betKindId.GetNoOfNumbers();
            var oddsValue = new List<OddsTableDetailModel>();
            for (var i = 0; i < noOfNumbers; i++)
            {
                oddsValue.Add(new OddsTableDetailModel
                {
                    Number = i,
                    OriginValue = defaultOddsValue.Buy
                });
            }
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
                f.CompanyPayouts = oddsStatsVal.CompanyPayout;
            });
            return new OddsTableModel
            {
                Match = runningMatch,
                Odds = oddsValue
            };
        }

        public async Task<MixedOddsTableModel> GetMixedOddsTableByBetKind(int betKindId)
        {
            var betKindIds = betKindId.BuildBetKinds();
            var betKindInMemoryRepository = _inMemoryUnitOfWork.GetRepository<IBetKindInMemoryRepository>();
            var betKinds = betKindInMemoryRepository.FindBy(f => betKindIds.Contains(f.Id)).ToDictionary(f => f.Id, f => f.Name);

            var runningMatch = await _runningMatchService.GetRunningMatch();
            if (runningMatch == null)
            {
                return new MixedOddsTableModel
                {
                    BetKinds = betKinds
                };
            }

            return new MixedOddsTableModel
            {
                Match = runningMatch,
                BetKinds = betKinds
            };
        }

        public async Task ChangeOddsValueOfOddsTable(ChangeOddsValueOfOddsTableModel model)
        {
            var runningMatch = await _runningMatchService.GetRunningMatch() ?? throw new NotFoundException();
            if (runningMatch.MatchId != model.MatchId) throw new NotFoundException();
            await _processOddsService.ChangeOddsValueOfOddsTable(model);
        }

        public async Task<Dictionary<long, LiveOddsModel>> GetLiveOdds(List<long> playerIds, int betKindId, long matchId, int regionId, int channelId)
        {
            if (betKindId != Enums.BetKind.FirstNorthern_Northern_LoLive.ToInt()) return new Dictionary<long, LiveOddsModel>();
            var betKindIds = new List<int> { Enums.BetKind.FirstNorthern_Northern_Lo.ToInt(), betKindId };

            var runningMatch = await _runningMatchService.GetRunningMatch();
            if (runningMatch == null || runningMatch.MatchId != matchId || runningMatch.State != MatchState.Running.ToInt()) return new Dictionary<long, LiveOddsModel>();
            if (!runningMatch.MatchResult.TryGetValue(regionId, out List<ResultByRegionModel> resultsOfChannel)) return new Dictionary<long, LiveOddsModel>();
            var resultsOfChannelDetail = resultsOfChannel.FirstOrDefault(f => f.ChannelId == channelId && f.IsLive);
            if (resultsOfChannelDetail == null) return new Dictionary<long, LiveOddsModel>();

            var rateOfOddsValue = await _processOddsService.GetRateOfOddsValue(runningMatch.MatchId, betKindIds);
            var allPlayerOdds = await GetMixedOddsBy(playerIds, betKindIds);   //  TODO: Need to read from cache

            var dictPlayerOdds = new Dictionary<long, LiveOddsModel>();
            foreach (var playerId in playerIds)
            {
                var playerOdds = allPlayerOdds.Where(f => f.PlayerId == playerId).ToList();
                var odds = _runningMatchService.GetOddsByPlayerForNorthern(playerId, playerOdds, rateOfOddsValue, runningMatch);
                if (odds.Count == 0) continue;
                if (!dictPlayerOdds.TryGetValue(playerId, out LiveOddsModel liveOdds))
                {
                    liveOdds = new LiveOddsModel
                    {
                        NoOfRemainingNumbers = resultsOfChannelDetail.NoOfRemainingNumbers
                    };
                    dictPlayerOdds[playerId] = liveOdds;
                }
                liveOdds.Odds.AddRange(odds);
            }
            return dictPlayerOdds;
        }

        public async Task<List<OddsByNumberModel>> GetInitialOdds(long playerId, int betKindId)
        {
            var betKindIds = betKindId.BuildBetKinds();
            var noOfNumbers = betKindId.GetNoOfNumbers();

            var runningMatch = await _runningMatchService.GetRunningMatch();
            var playerOdds = await GetOddsByListBetKind(playerId, betKindIds);    //  TODO: Need to read from cache

            if (betKindId == Enums.BetKind.FirstNorthern_Northern_LoXien.ToInt()) return await BuildInitialOddsXien(runningMatch, noOfNumbers, betKindIds, playerOdds);
            if (betKindId == Enums.BetKind.FirstNorthern_Northern_LoLive.ToInt()) return await BuildInitialOddsLoLive(runningMatch, noOfNumbers, betKindIds, playerOdds);

            return await BuildInitialOdds(runningMatch, noOfNumbers, betKindId, betKindIds, playerOdds);
        }

        private async Task<List<OddsByNumberModel>> BuildInitialOdds(MatchModel runningMatch, int noOfNumbers, int betKindId, List<int> betKindIds, List<PlayerOddsModel> playerOdds)
        {
            var rateOfOddsValue = runningMatch != null ? await _processOddsService.GetRateOfOddsValue(runningMatch.MatchId, betKindIds, noOfNumbers) : new Dictionary<int, Dictionary<int, decimal>>();
            var odds = new List<OddsByNumberModel>();
            for (var i = 0; i < noOfNumbers; i++)
            {
                var currentOdds = playerOdds.FirstOrDefault();
                var rateValue = _normalizeValueService.GetRateValue(betKindId, i, rateOfOddsValue);
                odds.Add(new OddsByNumberModel
                {
                    Number = i,
                    BetKinds = new List<OddsByBetKindModel>
                    {
                        new OddsByBetKindModel
                        {
                            Id = betKindId,
                            Buy = currentOdds == null ? 0m : _normalizeValueService.Normalize(currentOdds.Buy),
                            TotalRate = currentOdds == null ? 0m : _normalizeValueService.Normalize(rateValue)
                        }
                    }
                });
            }
            return odds;
        }

        private async Task<List<OddsByNumberModel>> BuildInitialOddsLoLive(MatchModel runningMatch, int noOfNumbers, List<int> betKindIds, List<PlayerOddsModel> playerOdds)
        {
            var rateOfOddsValue = runningMatch != null ? await _processOddsService.GetRateOfOddsValue(runningMatch.MatchId, betKindIds, noOfNumbers) : new Dictionary<int, Dictionary<int, decimal>>();
            var odds = new List<OddsByNumberModel>();
            for (var i = 0; i < noOfNumbers; i++)
            {
                var playerOddsForLo = playerOdds.FirstOrDefault(f => f.BetKindId == Enums.BetKind.FirstNorthern_Northern_Lo.ToInt());
                var playerOddsForLoLive = playerOdds.FirstOrDefault(f => f.BetKindId == Enums.BetKind.FirstNorthern_Northern_LoLive.ToInt());
                if (playerOddsForLo == null || playerOddsForLoLive == null) throw new BadRequestException();

                var rateValueOfLo = _normalizeValueService.GetRateValue(Enums.BetKind.FirstNorthern_Northern_Lo.ToInt(), i, rateOfOddsValue);

                var buyLo = _normalizeValueService.Normalize(playerOddsForLo.Buy) + _normalizeValueService.Normalize(rateValueOfLo);
                var buyLoLive = _normalizeValueService.Normalize(playerOddsForLoLive.Buy);

                var startBuyLoLive = buyLo;
                if (startBuyLoLive < buyLoLive) startBuyLoLive = buyLoLive;

                if (runningMatch != null) startBuyLoLive = _normalizeValueService.Normalize(_runningMatchService.GetLiveOdds(Enums.BetKind.FirstNorthern_Northern_LoLive.ToInt(), runningMatch, startBuyLoLive));

                odds.Add(new OddsByNumberModel
                {
                    Number = i,
                    BetKinds = new List<OddsByBetKindModel>
                    {
                        new OddsByBetKindModel
                        {
                            Id = Enums.BetKind.FirstNorthern_Northern_LoLive.ToInt(),
                            Buy = startBuyLoLive,
                            TotalRate = 0m
                        }
                    }
                });
            }
            return odds;
        }

        private async Task<List<OddsByNumberModel>> BuildInitialOddsXien(MatchModel runningMatch, int noOfNumbers, List<int> betKindIds, List<PlayerOddsModel> playerOdds)
        {
            var rateOfOddsValue = runningMatch != null ? await _processOddsService.GetMixedRateOfOddsValue(runningMatch.MatchId, betKindIds) : new Dictionary<int, decimal>();
            if (!rateOfOddsValue.TryGetValue(Enums.BetKind.FirstNorthern_Northern_Xien2.ToInt(), out decimal rateValueOfXien2)) rateValueOfXien2 = 0m;
            if (!rateOfOddsValue.TryGetValue(Enums.BetKind.FirstNorthern_Northern_Xien3.ToInt(), out decimal rateValueOfXien3)) rateValueOfXien3 = 0m;
            if (!rateOfOddsValue.TryGetValue(Enums.BetKind.FirstNorthern_Northern_Xien4.ToInt(), out decimal rateValueOfXien4)) rateValueOfXien4 = 0m;

            var odds = new List<OddsByNumberModel>();
            for (var i = 0; i < noOfNumbers; i++)
            {
                var xien2 = playerOdds.FirstOrDefault(f => f.BetKindId == Enums.BetKind.FirstNorthern_Northern_Xien2.ToInt());
                var xien3 = playerOdds.FirstOrDefault(f => f.BetKindId == Enums.BetKind.FirstNorthern_Northern_Xien3.ToInt());
                var xien4 = playerOdds.FirstOrDefault(f => f.BetKindId == Enums.BetKind.FirstNorthern_Northern_Xien4.ToInt());

                odds.Add(new OddsByNumberModel
                {
                    Number = i,
                    BetKinds = new List<OddsByBetKindModel>
                        {
                            new OddsByBetKindModel
                            {
                                Id = Enums.BetKind.FirstNorthern_Northern_Xien2.ToInt(),
                                Buy = xien2 == null ? 0m : _normalizeValueService.Normalize(xien2.Buy),
                                TotalRate = xien2 == null ? 0m : _normalizeValueService.Normalize(rateValueOfXien2)
                            },
                            new OddsByBetKindModel
                            {
                                Id = Enums.BetKind.FirstNorthern_Northern_Xien3.ToInt(),
                                Buy = xien3 == null ? 0m : _normalizeValueService.Normalize(xien3.Buy),
                                TotalRate = xien3 == null ? 0m : _normalizeValueService.Normalize(rateValueOfXien3)
                            },
                            new OddsByBetKindModel
                            {
                                Id = Enums.BetKind.FirstNorthern_Northern_Xien4.ToInt(),
                                Buy = xien4 == null ? 0m : _normalizeValueService.Normalize(xien4.Buy),
                                TotalRate = xien4 == null ? 0m : _normalizeValueService.Normalize(rateValueOfXien4)
                            }
                        }
                });
            }
            return odds;
        }
    }
}
