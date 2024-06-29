using HnMicro.Core.Helpers;
using HnMicro.Framework.Exceptions;
using HnMicro.Framework.Services;
using HnMicro.Module.Caching.ByRedis.Services;
using HnMicro.Modules.InMemory.UnitOfWorks;
using Lottery.Core.Configs;
using Lottery.Core.Contexts;
using Lottery.Core.Helpers;
using Lottery.Core.InMemory.BetKind;
using Lottery.Core.InMemory.Channel;
using Lottery.Core.Models.Match;
using Lottery.Core.Models.MatchResult;
using Lottery.Core.Models.Odds;
using Lottery.Core.Repositories.Match;
using Lottery.Core.Services.Others;
using Lottery.Core.UnitOfWorks;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace Lottery.Core.Services.Match
{
    public class RunningMatchService : LotteryBaseService<RunningMatchService>, IRunningMatchService
    {
        private readonly IInMemoryUnitOfWork _inMemoryUnitOfWork;
        private readonly INormalizeValueService _normalizeValueService;
        private readonly IRedisCacheService _redisCacheService;

        public RunningMatchService(ILogger<RunningMatchService> logger, IServiceProvider serviceProvider, IConfiguration configuration, IClockService clockService, ILotteryClientContext clientContext, ILotteryUow lotteryUow,
            IInMemoryUnitOfWork inMemoryUnitOfWork,
            INormalizeValueService normalizeValueService,
            IRedisCacheService redisCacheService) : base(logger, serviceProvider, configuration, clockService, clientContext, lotteryUow)
        {
            _inMemoryUnitOfWork = inMemoryUnitOfWork;
            _normalizeValueService = normalizeValueService;
            _redisCacheService = redisCacheService;
        }

        public async Task<MatchModel> GetRunningMatch(bool inCache = true)
        {
            if (inCache)
            {
                var dataCache = await _redisCacheService.HashGetAsync(CachingConfigs.RunningMatchKey, CachingConfigs.RedisConnectionForApp);
                if (dataCache == null || dataCache.Count == 0) return null;

                if (!dataCache.TryGetValue(nameof(MatchModel.MatchId), out string sMatchId)) return null;
                if (!long.TryParse(sMatchId, out long matchId) || matchId <= 0L) return null;

                if (!dataCache.TryGetValue(nameof(MatchModel.MatchCode), out string matchCode) || string.IsNullOrEmpty(matchCode)) return null;

                if (!dataCache.TryGetValue(nameof(MatchModel.KickoffTime), out string sKickoffTime)) return null;
                var kickoffTime = sKickoffTime.ToDateTime(CachingConfigs.RedisFormatDateTime);
                if (kickoffTime == null) return null;

                if (!dataCache.TryGetValue(nameof(MatchModel.CreatedAt), out string sCreatedAt)) return null;
                var createdAt = sCreatedAt.ToDateTime(CachingConfigs.RedisFormatDateTime);
                if (createdAt == null) return null;

                if (!dataCache.TryGetValue(nameof(MatchModel.State), out string sState)) return null;
                if (!int.TryParse(sState, out int state) || state < 0) return null;

                if (!dataCache.TryGetValue(nameof(MatchModel.MatchResult), out string sMatchResults) || string.IsNullOrEmpty(sMatchResults)) return null;
                var matchResult = Newtonsoft.Json.JsonConvert.DeserializeObject<Dictionary<int, List<ResultByRegionModel>>>(sMatchResults);

                return new MatchModel
                {
                    MatchId = matchId,
                    MatchCode = matchCode,
                    KickoffTime = kickoffTime.Value,
                    State = state,
                    CreatedAt = createdAt.Value,
                    MatchResult = matchResult
                };
            }

            var matchRepository = LotteryUow.GetRepository<IMatchRepository>();
            var runningMatch = await matchRepository.GetRunningMatch();
            if (runningMatch == null) return null;
            return new MatchModel
            {
                MatchId = runningMatch.MatchId,
                MatchCode = runningMatch.MatchCode,
                KickoffTime = runningMatch.KickOffTime,
                State = runningMatch.MatchState,
                CreatedAt = runningMatch.CreatedAt,
                MatchResult = GetMatchResults(runningMatch)
            };
        }

        private Dictionary<int, List<ResultByRegionModel>> GetMatchResults(Data.Entities.Match match)
        {
            var channelIds = match.MatchResults.Select(f => f.ChannelId).ToList();
            var channelInMemoryRepository = _inMemoryUnitOfWork.GetRepository<IChannelInMemoryRepository>();
            var channels = channelInMemoryRepository.FindBy(f => channelIds.Contains(f.Id)).ToList();
            var resultsByRegion = new Dictionary<int, List<ResultByRegionModel>>();
            foreach (var item in match.MatchResults)
            {
                if (!resultsByRegion.TryGetValue(item.RegionId, out List<ResultByRegionModel> resultsByRegionDetail))
                {
                    resultsByRegionDetail = new List<ResultByRegionModel>();
                    resultsByRegion[item.RegionId] = resultsByRegionDetail;
                }
                var itemChannel = channels.FirstOrDefault(f => f.Id == item.ChannelId);
                if (itemChannel == null) continue;

                var detailResults = DeserializeResults(item.Results);
                var noOfRemainingNumbers = CountNoOfRemainingNumbers(item.RegionId, detailResults);

                resultsByRegionDetail.Add(new ResultByRegionModel
                {
                    ChannelId = itemChannel.Id,
                    ChannelName = itemChannel.Name,
                    EnabledProcessTicket = item.EnabledProcessTicket,
                    IsLive = item.IsLive,
                    NoOfRemainingNumbers = noOfRemainingNumbers,
                    Prize = detailResults
                });
            }
            return resultsByRegion;
        }

        public List<PrizeResultModel> DeserializeResults(string results)
        {
            return Newtonsoft.Json.JsonConvert.DeserializeObject<List<PrizeResultModel>>(results);
        }

        public string SerializeResults(List<PrizeResultModel> results)
        {
            return Newtonsoft.Json.JsonConvert.SerializeObject(results);
        }

        public decimal GetLiveOdds(int betKindId, MatchModel match, decimal defaultOddsValue)
        {
            var betKindInMemoryRepository = _inMemoryUnitOfWork.GetRepository<IBetKindInMemoryRepository>();
            var betKind = betKindInMemoryRepository.FindById(betKindId) ?? throw new NotFoundException();
            if (!match.MatchResult.TryGetValue(betKind.RegionId, out List<ResultByRegionModel> regionResults)) throw new NotFoundException();

            var channelInMemoryRepository = _inMemoryUnitOfWork.GetRepository<IChannelInMemoryRepository>();
            var channel = channelInMemoryRepository.FindByRegionAndDayOfWeek(betKind.RegionId, match.KickoffTime.DayOfWeek) ?? throw new NotFoundException();

            var channelResult = regionResults.FirstOrDefault(f => f.ChannelId == channel.Id) ?? throw new NotFoundException();

            var startOfPosition = betKind.RegionId.GetStartOfPosition();
            var totalNumber = TotalNumbers(betKind.RegionId, channelResult.Prize);

            var stop = false;
            var position = 0;
            foreach (var itemPrize in channelResult.Prize)
            {
                foreach (var itemResult in itemPrize.Results)
                {
                    if (itemResult.Position < startOfPosition) continue;
                    if (!itemResult.AllowProcessTicket)
                    {
                        position++;
                        continue;
                    }

                    stop = true;
                    break;
                }
                if (!stop) continue;
                break;
            }
            return defaultOddsValue - (position * GetAmountDecrement(defaultOddsValue, totalNumber));
        }

        private decimal GetAmountDecrement(decimal defaultOddsValue, int noOfNumbers)
        {
            return 1m * defaultOddsValue / noOfNumbers;
        }

        public List<OddsByNumberModel> GetOddsByPlayerForNorthern(long playerId, List<PlayerOddsModel> playerOdds, Dictionary<int, Dictionary<int, decimal>> rateOfOddsValue, MatchModel runningMatch)
        {
            var oddsMessages = new List<OddsByNumberModel>();
            var betKindId = Enums.BetKind.FirstNorthern_Northern_LoLive.ToInt();
            for (var i = 0; i < 100; i++)
            {
                var playerOddsForLo = playerOdds.FirstOrDefault(f => f.BetKindId == Enums.BetKind.FirstNorthern_Northern_Lo.ToInt());
                var playerOddsForLoLive = playerOdds.FirstOrDefault(f => f.BetKindId == betKindId);
                if (playerOddsForLo == null || playerOddsForLoLive == null) throw new BadRequestException();

                var rateValueOfLo = _normalizeValueService.GetRateValue(Enums.BetKind.FirstNorthern_Northern_Lo.ToInt(), i, rateOfOddsValue);
                //var rateValueOfLoLive = GetRateValue(BetKind.FirstNorthern_Northern_LoLive.ToInt(), i, rateOfOddsValue);

                var buyLo = _normalizeValueService.Normalize(playerOddsForLo.Buy) + _normalizeValueService.Normalize(rateValueOfLo);
                var buyLoLive = _normalizeValueService.Normalize(playerOddsForLoLive.Buy);  // + _normalizeValueService.Normalize(rateValueOfLo);

                var startBuyLoLive = buyLo;
                if (startBuyLoLive < buyLoLive) startBuyLoLive = buyLoLive;

                //  Calculate Buy by Position
                startBuyLoLive = _normalizeValueService.Normalize(GetLiveOdds(betKindId, runningMatch, startBuyLoLive));

                oddsMessages.Add(new OddsByNumberModel
                {
                    Number = i,
                    BetKinds = new List<OddsByBetKindModel>
                        {
                            new OddsByBetKindModel
                            {
                                Id = betKindId,
                                Buy = startBuyLoLive,
                                TotalRate = 0m
                            }
                        }
                });
            }
            return oddsMessages;
        }

        public (PrizeResultModel, PrizeResultDetailModel) GetCurrentPrize(int regionId, List<PrizeResultModel> listPrize)
        {
            var startOfPosition = regionId.GetStartOfPosition();
            var results = listPrize.SelectMany(f => f.Results).OrderBy(f => f.Position).ToList();
            var currentPosition = results.FirstOrDefault(f => f.Position >= startOfPosition && f.AllowProcessTicket);
            if (currentPosition == null) return (null, null);
            return (listPrize.FirstOrDefault(f => f.Results.Any(f1 => f1.Position == currentPosition.Position)), currentPosition);
        }

        public int CountNoOfRemainingNumbers(int regionId, List<PrizeResultModel> listPrize)
        {
            var startOfPosition = regionId.GetStartOfPosition();
            return listPrize.SelectMany(f => f.Results).Where(f => f.Position >= startOfPosition).Count(f => f.AllowProcessTicket);
        }

        public int TotalNumbers(int regionId, List<PrizeResultModel> listPrize)
        {
            var startOfPosition = regionId.GetStartOfPosition();
            return listPrize.SelectMany(f => f.Results).Where(f => f.Position >= startOfPosition).Count();
        }
    }
}
