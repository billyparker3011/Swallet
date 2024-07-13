using HnMicro.Core.Helpers;
using HnMicro.Framework.Exceptions;
using HnMicro.Framework.Services;
using HnMicro.Module.Caching.ByRedis.Services;
using Lottery.Core.Configs;
using Lottery.Core.Contexts;
using Lottery.Core.Helpers;
using Lottery.Core.Models.Odds;
using Lottery.Core.Services.Pubs;
using Lottery.Core.UnitOfWorks;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace Lottery.Core.Services.Odds
{
    public class ProcessOddsService : LotteryBaseService<ProcessOddsService>, IProcessOddsService
    {
        private readonly IRedisCacheService _redisCacheService;
        private readonly IPublishCommonService _publishCommonService;

        public ProcessOddsService(ILogger<ProcessOddsService> logger, IServiceProvider serviceProvider, IConfiguration configuration, IClockService clockService, ILotteryClientContext clientContext, ILotteryUow lotteryUow,
            IRedisCacheService redisCacheService,
            IPublishCommonService publishCommonService) : base(logger, serviceProvider, configuration, clockService, clientContext, lotteryUow)
        {
            _redisCacheService = redisCacheService;
            _publishCommonService = publishCommonService;
        }

        public async Task<Dictionary<int, OddsStatsModel>> CalculateStats(long matchId, int betKindId)
        {
            var noOfNumbers = betKindId.GetNoOfNumbers();
            var data = new Dictionary<int, OddsStatsModel>();
            for (var i = 0; i < noOfNumbers; i++)
            {
                var pointStatsKey = matchId.GetPointStatsKeyByMatchBetKindNumber(betKindId, i);
                var pointStats = await _redisCacheService.HashGetFieldsAsync(pointStatsKey.MainKey, new List<string> { pointStatsKey.SubKey }, CachingConfigs.RedisConnectionForApp);
                if (!pointStats.TryGetValue(pointStatsKey.SubKey, out string sPointStatsValue) || !decimal.TryParse(sPointStatsValue, out decimal pointStatsValue))
                    pointStatsValue = 0m;

                var payoutStatsKey = matchId.GetPayoutStatsKeyByMatchBetKindNumber(betKindId, i);
                var payoutStats = await _redisCacheService.HashGetFieldsAsync(payoutStatsKey.MainKey, new List<string> { payoutStatsKey.SubKey }, CachingConfigs.RedisConnectionForApp);
                if (!payoutStats.TryGetValue(payoutStatsKey.SubKey, out string sPayoutStatsValue) || !decimal.TryParse(sPayoutStatsValue, out decimal payoutStatsValue))
                    payoutStatsValue = 0m;

                var rateStatsKey = matchId.GetRateStatsKeyByMatchBetKindNumber(betKindId, i);
                var rateStats = await _redisCacheService.HashGetFieldsAsync(rateStatsKey.MainKey, new List<string> { rateStatsKey.SubKey }, CachingConfigs.RedisConnectionForApp);
                if (!rateStats.TryGetValue(rateStatsKey.SubKey, out string sRateStatsValue) || !decimal.TryParse(sRateStatsValue, out decimal rateStatsValue))
                    rateStatsValue = 0m;

                var companyPayoutStatsKey = matchId.GetCompanyPayoutStatsKeyByMatchBetKindNumber(betKindId, i);
                var companyPayoutStats = await _redisCacheService.HashGetFieldsAsync(companyPayoutStatsKey.MainKey, new List<string> { companyPayoutStatsKey.SubKey }, CachingConfigs.RedisConnectionForApp);
                if (!companyPayoutStats.TryGetValue(companyPayoutStatsKey.SubKey, out string sCompanyPayoutStatsValue) || !decimal.TryParse(sCompanyPayoutStatsValue, out decimal companyPayoutStatsValue))
                    companyPayoutStatsValue = 0m;

                data[i] = new OddsStatsModel
                {
                    Payout = payoutStatsValue,
                    Point = pointStatsValue,
                    Rate = rateStatsValue,
                    CompanyPayout = companyPayoutStatsValue
                };
            }
            return data;
        }

        public async Task<Dictionary<int, Dictionary<int, decimal>>> GetRateOfOddsValue(long matchId, List<int> betKindIds, int noOfNumbers = 100)
        {
            var data = new Dictionary<int, Dictionary<int, decimal>>();
            foreach (var betKindId in betKindIds)
            {
                data[betKindId] = await GetRateOfOddsValueBetKind(matchId, betKindId);
            }
            return data;
        }

        private async Task<Dictionary<int, decimal>> GetRateOfOddsValueBetKind(long matchId, int betKindId, int noOfNumbers = 100)
        {
            var data = new Dictionary<int, decimal>();
            for (var i = 0; i < noOfNumbers; i++)
            {
                var rateStatsKey = matchId.GetRateStatsKeyByMatchBetKindNumber(betKindId, i);
                var rateStats = await _redisCacheService.HashGetFieldsAsync(rateStatsKey.MainKey, new List<string> { rateStatsKey.SubKey }, CachingConfigs.RedisConnectionForApp);
                if (!rateStats.TryGetValue(rateStatsKey.SubKey, out string sRateStatsValue) || !decimal.TryParse(sRateStatsValue, out decimal rateStatsValue))
                {
                    rateStatsValue = 0m;
                }
                data[i] = rateStatsValue;
            }
            return data;
        }

        public async Task ChangeOddsValueOfOddsTable(ChangeOddsValueOfOddsTableModel model)
        {
            var rateStatsKey = model.MatchId.GetRateStatsKeyByMatchBetKindNumber(model.BetKindId, model.Number);
            var rateStats = await _redisCacheService.HashGetFieldsAsync(rateStatsKey.MainKey, new List<string> { rateStatsKey.SubKey }, CachingConfigs.RedisConnectionForApp);
            if (!rateStats.TryGetValue(rateStatsKey.SubKey, out string sRateStats) || !decimal.TryParse(sRateStats, out decimal rateStatsVal))
            {
                rateStatsVal = 0m;
            }
            var rate = Math.Abs(model.Rate);
            if (!model.Increment && (rateStatsVal - rate < 0m)) throw new BadRequestException();
            rateStatsVal = model.Increment ? rateStatsVal + rate : rateStatsVal - rate;

            await _redisCacheService.HashSetFieldsAsync(rateStatsKey.MainKey, new Dictionary<string, string> { { rateStatsKey.SubKey, rateStatsVal.ToString() } }, rateStatsKey.TimeSpan, CachingConfigs.RedisConnectionForApp);

            await _publishCommonService.PublishOddsValue(new RateOfOddsValueModel
            {
                MatchId = model.MatchId,
                BetKindId = model.BetKindId,
                Number = model.Number,
                TotalRate = rateStatsVal
            });
        }

        public void UpdateRateOfOddsValue(long matchId, int betKindId, Dictionary<int, decimal> rate)
        {
            foreach (var item in rate)
            {
                var rateStatsKey = matchId.GetRateStatsKeyByMatchBetKindNumber(betKindId, item.Key);
                var rateStats = _redisCacheService.HashGetFields(rateStatsKey.MainKey, new List<string> { rateStatsKey.SubKey }, CachingConfigs.RedisConnectionForApp);
                if (!rateStats.TryGetValue(rateStatsKey.SubKey, out string sRateStatsValue) || !decimal.TryParse(sRateStatsValue, out decimal rateStatsValue)) rateStatsValue = 0m;
                rateStatsValue += item.Value;

                _redisCacheService.HashSet(rateStatsKey.MainKey, new Dictionary<string, string>
                {
                    { rateStatsKey.SubKey, rateStatsValue.ToString() }
                }, rateStatsKey.TimeSpan, CachingConfigs.RedisConnectionForApp);

                _publishCommonService.PublishOddsValueSingle(new RateOfOddsValueModel
                {
                    BetKindId = betKindId,
                    MatchId = matchId,
                    Number = item.Key,
                    TotalRate = item.Value
                });
            }
        }

        public async Task<List<MixedOddsTableDetailModel>> GetMixedOddsTableDetail(long matchId, int betKindId, int top)
        {
            var pointMixStatsByMatchKey = matchId.GetPointMixedStatsKeyByMatchBetKind(betKindId);
            var payoutMixStatsByMatchKey = matchId.GetPayoutMixedStatsKeyByMatchBetKind(betKindId);
            var companyPayoutMixStatsByMatchKey = matchId.GetCompanyPayoutMixedStatsKeyByMatchBetKind(betKindId);

            var dictPairs = await _redisCacheService.SortedSetRangeByScoreWithScoresInDecimalAsync(companyPayoutMixStatsByMatchKey.MainKey, decimal.MinValue, decimal.MaxValue, StackExchange.Redis.Exclude.None, StackExchange.Redis.Order.Descending, 0, 1L * top, CachingConfigs.RedisConnectionForApp);
            var dictPoints = await _redisCacheService.SortedSetRangeByScoreWithScoresInDecimalAsync(pointMixStatsByMatchKey.MainKey, decimal.MinValue, decimal.MaxValue, StackExchange.Redis.Exclude.None, StackExchange.Redis.Order.Ascending, 0, -1L, CachingConfigs.RedisConnectionForApp);
            var dictPayouts = await _redisCacheService.SortedSetRangeByScoreWithScoresInDecimalAsync(payoutMixStatsByMatchKey.MainKey, decimal.MinValue, decimal.MaxValue, StackExchange.Redis.Exclude.None, StackExchange.Redis.Order.Ascending, 0, -1L, CachingConfigs.RedisConnectionForApp);
            var data = dictPairs.Select(f => new MixedOddsTableDetailModel
            {
                Pair = f.Key,
                CompanyPayout = f.Value
            }).ToList();
            data.ForEach(f =>
            {
                if (dictPoints.TryGetValue(f.Pair, out decimal point)) f.Point = point;
                if (dictPayouts.TryGetValue(f.Pair, out decimal payout)) f.Payout = payout;
            });
            return data;
        }

        public async Task<List<MixedOddsTableRelatedBetKindModel>> GetMixedOddsTableRelatedBetKind(long matchId, int betKindId, int top)
        {
            if (betKindId != Enums.BetKind.FirstNorthern_Northern_LoXien.ToInt()) return new List<MixedOddsTableRelatedBetKindModel>();
            betKindId = Enums.BetKind.FirstNorthern_Northern_Lo.ToInt();
            var noOfNumbers = betKindId.GetNoOfNumbers();

            var data = new List<MixedOddsTableRelatedBetKindModel>();
            for (var i = 0; i < noOfNumbers; i++)
            {
                var pointStatsKey = matchId.GetPointStatsKeyByMatchBetKindNumber(betKindId, i);
                var pointStats = await _redisCacheService.HashGetFieldsAsync(pointStatsKey.MainKey, new List<string> { pointStatsKey.SubKey }, CachingConfigs.RedisConnectionForApp);
                if (!pointStats.TryGetValue(pointStatsKey.SubKey, out string sPointStatsValue) || !decimal.TryParse(sPointStatsValue, out decimal pointStatsValue))
                    pointStatsValue = 0m;
                if (pointStatsValue <= 0m) continue;

                var payoutStatsKey = matchId.GetPayoutStatsKeyByMatchBetKindNumber(betKindId, i);
                var payoutStats = await _redisCacheService.HashGetFieldsAsync(payoutStatsKey.MainKey, new List<string> { payoutStatsKey.SubKey }, CachingConfigs.RedisConnectionForApp);
                if (!payoutStats.TryGetValue(payoutStatsKey.SubKey, out string sPayoutStatsValue) || !decimal.TryParse(sPayoutStatsValue, out decimal payoutStatsValue))
                    payoutStatsValue = 0m;

                var companyPayoutStatsKey = matchId.GetCompanyPayoutStatsKeyByMatchBetKindNumber(betKindId, i);
                var companyPayoutStats = await _redisCacheService.HashGetFieldsAsync(companyPayoutStatsKey.MainKey, new List<string> { companyPayoutStatsKey.SubKey }, CachingConfigs.RedisConnectionForApp);
                if (!companyPayoutStats.TryGetValue(companyPayoutStatsKey.SubKey, out string sCompanyPayoutStatsValue) || !decimal.TryParse(sCompanyPayoutStatsValue, out decimal companyPayoutStatsValue))
                    companyPayoutStatsValue = 0m;

                var payoutStatsKey = matchId.GetPayoutStatsKeyByMatchBetKindNumber(betKindId, i);
                var payoutStats = await _redisCacheService.HashGetFieldsAsync(payoutStatsKey.MainKey, new List<string> { payoutStatsKey.SubKey }, CachingConfigs.RedisConnectionForApp);
                if (!payoutStats.TryGetValue(payoutStatsKey.SubKey, out string sPayoutStatsValue) || !decimal.TryParse(sPayoutStatsValue, out decimal payoutStatsValue))
                    payoutStatsValue = 0m;

                var pointStatsKey = matchId.GetPointStatsKeyByMatchBetKindNumber(betKindId, i);
                var pointStats = await _redisCacheService.HashGetFieldsAsync(pointStatsKey.MainKey, new List<string> { pointStatsKey.SubKey }, CachingConfigs.RedisConnectionForApp);
                if (!pointStats.TryGetValue(pointStatsKey.SubKey, out string sPointStatsValue) || !decimal.TryParse(sPointStatsValue, out decimal pointStatsValue))
                    pointStatsValue = 0m;

                data.Add(new MixedOddsTableRelatedBetKindModel
                {
                    Number = i,
                    Payout = payoutStatsValue,
                    Point = pointStatsValue,
                    CompanyPayout = companyPayoutStatsValue
                });
            }
            return data.OrderByDescending(f => f.CompanyPayout).Take(top).ToList();
        }
    }
}
