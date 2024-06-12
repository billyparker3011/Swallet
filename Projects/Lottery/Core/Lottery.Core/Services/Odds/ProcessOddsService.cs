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
            var data = new Dictionary<int, OddsStatsModel>();
            for (var i = 0; i < 100; i++)
            {
                var pointStatsKey = matchId.GetPointStatsKeyByMatchBetKindNumber(betKindId, i);
                var pointStats = await _redisCacheService.HashGetFieldsAsync(pointStatsKey.MainKey, new List<string> { pointStatsKey.SubKey }, CachingConfigs.RedisConnectionForApp);
                if (!pointStats.TryGetValue(pointStatsKey.SubKey, out string sPointStatsValue) || !decimal.TryParse(sPointStatsValue, out decimal pointStatsValue))
                {
                    pointStatsValue = 0m;
                }

                var payoutStatsKey = matchId.GetPayoutStatsKeyByMatchBetKindNumber(betKindId, i);
                var payoutStats = await _redisCacheService.HashGetFieldsAsync(payoutStatsKey.MainKey, new List<string> { payoutStatsKey.SubKey }, CachingConfigs.RedisConnectionForApp);
                if (!payoutStats.TryGetValue(payoutStatsKey.SubKey, out string sPayoutStatsValue) || !decimal.TryParse(sPayoutStatsValue, out decimal payoutStatsValue))
                {
                    payoutStatsValue = 0m;
                }

                var rateStatsKey = matchId.GetRateStatsKeyByMatchBetKindNumber(betKindId, i);
                var rateStats = await _redisCacheService.HashGetFieldsAsync(rateStatsKey.MainKey, new List<string> { rateStatsKey.SubKey }, CachingConfigs.RedisConnectionForApp);
                if (!rateStats.TryGetValue(rateStatsKey.SubKey, out string sRateStatsValue) || !decimal.TryParse(sRateStatsValue, out decimal rateStatsValue))
                {
                    rateStatsValue = 0m;
                }

                data[i] = new OddsStatsModel
                {
                    Payout = payoutStatsValue,
                    Point = pointStatsValue,
                    Rate = rateStatsValue
                };
            }
            return data;
        }

        public async Task<Dictionary<int, Dictionary<int, decimal>>> GetRateOfOddsValue(long matchId, List<int> betKindIds)
        {
            var data = new Dictionary<int, Dictionary<int, decimal>>();
            foreach (var betKindId in betKindIds)
            {
                data[betKindId] = await GetRateOfOddsValueBetKind(matchId, betKindId);
            }
            return data;
        }

        private async Task<Dictionary<int, decimal>> GetRateOfOddsValueBetKind(long matchId, int betKindId)
        {
            var data = new Dictionary<int, decimal>();
            for (var i = 0; i < 100; i++)
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
    }
}
