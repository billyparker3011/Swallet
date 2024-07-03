using HnMicro.Framework.Services;
using HnMicro.Module.Caching.ByRedis.Services;
using Lottery.Core.Configs;
using Lottery.Core.Contexts;
using Lottery.Core.Helpers;
using Lottery.Core.Models.Odds;
using Lottery.Core.UnitOfWorks;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace Lottery.Core.Services.Odds
{
    public class NumberService : LotteryBaseService<NumberService>, INumberService
    {
        private readonly IRedisCacheService _redisCacheService;

        public NumberService(ILogger<NumberService> logger, IServiceProvider serviceProvider, IConfiguration configuration, IClockService clockService, ILotteryClientContext clientContext, ILotteryUow lotteryUow,
            IRedisCacheService redisCacheService) : base(logger, serviceProvider, configuration, clockService, clientContext, lotteryUow)
        {
            _redisCacheService = redisCacheService;
        }

        public async Task AddSuspendedNumbers(AddSuspendedNumbersModel model)
        {
            var suspendedNumbersKey = model.MatchId.GetSuspendedNumbersByMatchAndBetKind(model.BetKindId);
            foreach (var item in model.Numbers)
                await _redisCacheService.SetAddAsync(suspendedNumbersKey.MainKey, item, suspendedNumbersKey.TimeSpan, CachingConfigs.RedisConnectionForApp);
        }

        public async Task DeleteSuspendedNumber(DeleteSuspendedNumberModel model)
        {
            var suspendedNumbersKey = model.MatchId.GetSuspendedNumbersByMatchAndBetKind(model.BetKindId);
            await _redisCacheService.SetRemoveAsync(suspendedNumbersKey.MainKey, new List<int> { model.Number }, CachingConfigs.RedisConnectionForApp);
        }

        public async Task<List<int>> GetSuspendedNumbersByMatchAndBetKind(long matchId, int betKindId)
        {
            var suspendedNumbersKey = matchId.GetSuspendedNumbersByMatchAndBetKind(betKindId);
            return await _redisCacheService.SetMembersAsync<int>(suspendedNumbersKey.MainKey, CachingConfigs.RedisConnectionForApp);
        }
    }
}
