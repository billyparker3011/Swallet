using HnMicro.Framework.Exceptions;
using HnMicro.Framework.Services;
using HnMicro.Module.Caching.ByRedis.Services;
using HnMicro.Modules.InMemory.UnitOfWorks;
using Lottery.Core.Configs;
using Lottery.Core.Contexts;
using Lottery.Core.Helpers;
using Lottery.Core.InMemory.BetKind;
using Lottery.Core.Models.Setting;
using Lottery.Core.Repositories.Player;
using Lottery.Core.UnitOfWorks;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace Lottery.Core.Services.Player
{
    public class PlayerSettingService : LotteryBaseService<PlayerSettingService>, IPlayerSettingService
    {
        private readonly IInMemoryUnitOfWork _inMemoryUnitOfWork;
        private readonly IRedisCacheService _cacheService;

        public PlayerSettingService(ILogger<PlayerSettingService> logger, IServiceProvider serviceProvider, IConfiguration configuration, IClockService clockService, ILotteryClientContext clientContext, ILotteryUow lotteryUow,
            IInMemoryUnitOfWork inMemoryUnitOfWork,
            IRedisCacheService cacheService) : base(logger, serviceProvider, configuration, clockService, clientContext, lotteryUow)
        {
            _inMemoryUnitOfWork = inMemoryUnitOfWork;
            _cacheService = cacheService;
        }

        public async Task<(BetSettingModel, bool)> GetBetSettings(long playerId, int betKindId)
        {
            var refreshCache = false;
            var betSetting = await GetBetSettingInCache(playerId, betKindId);
            if (betSetting == null)
            {
                refreshCache = true;
                betSetting = await GetBetSettingInDb(playerId, betKindId);
            }
            return (betSetting, refreshCache);
        }

        public async Task<(Dictionary<int, BetSettingModel>, bool)> GetBetSettings(long playerId, List<int> betKindIds)
        {
            var refreshCache = false;
            var dictBetSettings = new Dictionary<int, BetSettingModel>();
            foreach (var betKindId in betKindIds)
            {
                var betSetting = await GetBetSettingInCache(playerId, betKindId);
                if (betSetting == null)
                {
                    refreshCache = true;
                    betSetting = await GetBetSettingInDb(playerId, betKindId);
                }
                dictBetSettings[betKindId] = betSetting;
            }
            return (dictBetSettings, refreshCache);
        }

        public async Task<PlayerBetSettingModel> GetMyBetSettingByBetKind(int betKindId)
        {
            var playerId = ClientContext.Player.PlayerId;
            (var setting, var refreshSettingCache) = await GetBetSettings(playerId, betKindId);
            if (setting == null) throw new BadRequestException(ErrorCodeHelper.ProcessTicket.CannotReadBetSetting);
            if (refreshSettingCache) await BuildSettingByBetKindCache(playerId, betKindId, setting);

            var betKindInMemoryRepository = _inMemoryUnitOfWork.GetRepository<IBetKindInMemoryRepository>();
            var betKind = betKindInMemoryRepository.FindById(betKindId);
            if (betKind == null) throw new BadRequestException(ErrorCodeHelper.ProcessTicket.CannotReadBetSetting);

            return new PlayerBetSettingModel
            {
                MinBet = setting.MinBet,
                MaxBet = setting.MaxBet,
                MaxPerNumber = setting.MaxPerNumber,
                OddsValue = setting.OddsValue,
                RegionId = betKind.RegionId,
                Award = betKind.Award
            };
        }

        public async Task<Dictionary<int, BetSettingModel>> GetMyMixedBetSettingByBetKind(int betKindId)
        {
            var betKindInMemoryRepository = _inMemoryUnitOfWork.GetRepository<IBetKindInMemoryRepository>();
            var betKind = betKindInMemoryRepository.FindBy(f => f.Id == betKindId && f.IsMixed == true && f.CorrelationBetKindIds.Count > 0).FirstOrDefault();
            if (betKind == null) return new Dictionary<int, BetSettingModel>();

            var playerId = ClientContext.Player.PlayerId;
            (var settings, var refreshSettingCache) = await GetBetSettings(playerId, betKind.CorrelationBetKindIds);
            if (refreshSettingCache) await BuildSettingByBetKindCache(playerId, settings);
            return settings;
        }

        private async Task<BetSettingModel> GetBetSettingInDb(long playerId, int betKindId)
        {
            var playerOddRepository = LotteryUow.GetRepository<IPlayerOddRepository>();
            var playerOdd = await playerOddRepository.GetBetSettingByPlayerAndBetKind(playerId, betKindId);
            if (playerOdd == null) return null;
            return new BetSettingModel
            {
                MinBet = playerOdd.MinBet,
                MaxBet = playerOdd.MaxBet,
                MaxPerNumber = playerOdd.MaxPerNumber,
                OddsValue = playerOdd.Buy
            };
        }

        private async Task<BetSettingModel> GetBetSettingInCache(long playerId, int betKindId)
        {
            var minBetKey = playerId.GetMinBetPlayer(betKindId);
            var maxBetKey = playerId.GetMaxBetPlayer(betKindId);
            var maxPerNumberKey = playerId.GetMaxPerNumberPlayer(betKindId);
            var oddsKey = playerId.GetPlayerOddByBetKind(betKindId);

            var minBet = await _cacheService.HashGetFieldsAsync(minBetKey.MainKey, new List<string> { minBetKey.SubKey }, CachingConfigs.RedisConnectionForApp);
            var maxBet = await _cacheService.HashGetFieldsAsync(maxBetKey.MainKey, new List<string> { maxBetKey.SubKey }, CachingConfigs.RedisConnectionForApp);
            var maxPerNumber = await _cacheService.HashGetFieldsAsync(maxPerNumberKey.MainKey, new List<string> { maxPerNumberKey.SubKey }, CachingConfigs.RedisConnectionForApp);
            var odds = await _cacheService.HashGetFieldsAsync(oddsKey.MainKey, new List<string> { oddsKey.SubKey }, CachingConfigs.RedisConnectionForApp);
            if (minBet.Count == 0 || maxBet.Count == 0 || maxPerNumber.Count == 0 || odds.Count == 0) return null;

            var sMinBetValue = minBet.FirstOrDefault().Value;
            var sMaxBetValue = maxBet.FirstOrDefault().Value;
            var sMaxPerNumberValue = maxPerNumber.FirstOrDefault().Value;
            var sOddsValue = odds.FirstOrDefault().Value;
            if (!decimal.TryParse(sMinBetValue, out decimal minBetValue) ||
                !decimal.TryParse(sMaxBetValue, out decimal maxBetValue) ||
                !decimal.TryParse(sMaxPerNumberValue, out decimal maxPerNumberValue) ||
                !decimal.TryParse(sOddsValue, out decimal oddsValue)) return null;
            return new BetSettingModel
            {
                MinBet = minBetValue,
                MaxBet = maxBetValue,
                MaxPerNumber = maxPerNumberValue,
                OddsValue = oddsValue
            };
        }

        public async Task BuildSettingByBetKindCache(long playerId, int betKindId, BetSettingModel setting)
        {
            await InternalBuildSettingByBetKindCache(playerId, betKindId, setting);
        }

        private async Task InternalBuildSettingByBetKindCache(long playerId, int betKindId, BetSettingModel setting)
        {
            var minBetKey = playerId.GetMinBetPlayer(betKindId);
            await _cacheService.HashSetAsync(minBetKey.MainKey, new Dictionary<string, string>
            {
                { minBetKey.SubKey, setting.MinBet.ToString() }
            }, minBetKey.TimeSpan == TimeSpan.Zero ? null : minBetKey.TimeSpan, CachingConfigs.RedisConnectionForApp);

            var maxBetKey = playerId.GetMaxBetPlayer(betKindId);
            await _cacheService.HashSetAsync(maxBetKey.MainKey, new Dictionary<string, string>
            {
                { maxBetKey.SubKey, setting.MaxBet.ToString() }
            }, maxBetKey.TimeSpan == TimeSpan.Zero ? null : maxBetKey.TimeSpan, CachingConfigs.RedisConnectionForApp);

            var maxPerNumberKey = playerId.GetMaxPerNumberPlayer(betKindId);
            await _cacheService.HashSetAsync(maxPerNumberKey.MainKey, new Dictionary<string, string>
            {
                { maxPerNumberKey.SubKey, setting.MaxPerNumber.ToString() }
            }, maxPerNumberKey.TimeSpan == TimeSpan.Zero ? null : maxPerNumberKey.TimeSpan, CachingConfigs.RedisConnectionForApp);

            var oddsKey = playerId.GetPlayerOddByBetKind(betKindId);
            await _cacheService.HashSetAsync(oddsKey.MainKey, new Dictionary<string, string>
            {
                { oddsKey.SubKey, setting.OddsValue.ToString() }
            }, oddsKey.TimeSpan == TimeSpan.Zero ? null : oddsKey.TimeSpan, CachingConfigs.RedisConnectionForApp);
        }

        public async Task BuildSettingByBetKindCache(long playerId, Dictionary<int, BetSettingModel> settings)
        {
            foreach (var item in settings)
            {
                if (item.Value == null) continue;
                await InternalBuildSettingByBetKindCache(playerId, item.Key, item.Value);
            }
        }
    }
}
