using HnMicro.Framework.Services;
using HnMicro.Module.Caching.ByRedis.Services;
using HnMicro.Modules.InMemory.UnitOfWorks;
using Lottery.Core.Configs;
using Lottery.Core.Contexts;
using Lottery.Core.Helpers;
using Lottery.Core.InMemory.Odds;
using Lottery.Core.Models.Odds;
using Lottery.Core.Models.Outs;
using Lottery.Core.Models.Setting;
using Lottery.Core.Repositories.Player;
using Lottery.Core.Services.Odds;
using Lottery.Core.UnitOfWorks;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace Lottery.Core.Services.Caching.Player
{
    public class ProcessTicketService : LotteryBaseService<ProcessTicketService>, IProcessTicketService
    {
        private readonly IRedisCacheService _cacheService;
        private readonly IInMemoryUnitOfWork _inMemoryUnitOfWork;
        private readonly IOddsService _oddService;

        public ProcessTicketService(ILogger<ProcessTicketService> logger, IServiceProvider serviceProvider, IConfiguration configuration, IClockService clockService, ILotteryClientContext clientContext, ILotteryUow lotteryUow,
            IRedisCacheService cacheService,
            IInMemoryUnitOfWork inMemoryUnitOfWork,
            IOddsService oddService) : base(logger, serviceProvider, configuration, clockService, clientContext, lotteryUow)
        {
            _cacheService = cacheService;
            _inMemoryUnitOfWork = inMemoryUnitOfWork;
            _oddService = oddService;
        }

        public async Task BuildGivenCreditCache(long playerId, decimal credit)
        {
            var givenCreditKey = playerId.GetPlayerGivenCredit();
            await _cacheService.HashSetFieldsAsync(givenCreditKey.MainKey, new Dictionary<string, string>
            {
                { givenCreditKey.SubKey, credit.ToString() }
            }, givenCreditKey.TimeSpan == TimeSpan.Zero ? null : givenCreditKey.TimeSpan, CachingConfigs.RedisConnectionForApp);
        }

        public async Task BuildOutsByMatchCache(long playerId, long matchId, decimal totalOuts)
        {
            var outsKey = playerId.GetPlayerOutsByMatch(matchId);
            await _cacheService.HashSetFieldsAsync(outsKey.MainKey, new Dictionary<string, string>
            {
                { outsKey.SubKey, totalOuts.ToString() }
            }, outsKey.TimeSpan == TimeSpan.Zero ? null : outsKey.TimeSpan, CachingConfigs.RedisConnectionForApp);
        }

        public async Task BuildOutsByMatchAndNumbersCache(long playerId, long matchId, Dictionary<int, decimal> pointsByMatchAndNumbers, Dictionary<int, decimal> pointByNumbers)
        {
            foreach (var item in pointByNumbers)
            {
                var pointsByMatchAndNumberKey = playerId.GetPlayerPointsByMatchAndNumber(matchId, item.Key);
                if (!pointsByMatchAndNumbers.TryGetValue(item.Key, out decimal oldVal)) oldVal = 0m;
                var newVal = item.Value + oldVal;
                await _cacheService.HashSetFieldsAsync(pointsByMatchAndNumberKey.MainKey, new Dictionary<string, string>
                {
                    { pointsByMatchAndNumberKey.SubKey, newVal.ToString() }
                }, pointsByMatchAndNumberKey.TimeSpan == TimeSpan.Zero ? null : pointsByMatchAndNumberKey.TimeSpan, CachingConfigs.RedisConnectionForApp);
            }
        }

        public async Task BuildStatsByMatchBetKindAndNumbers(long matchId, int betKindId, Dictionary<int, decimal> pointByNumbers, Dictionary<int, decimal> payoutByNumbers)
        {
            //  Point
            foreach (var item in pointByNumbers)
            {
                var pointStatsByMatchAndNumbersKey = matchId.GetPointStatsKeyByMatchBetKindNumber(betKindId, item.Key);
                var vals = await _cacheService.HashGetFieldsAsync(pointStatsByMatchAndNumbersKey.MainKey, new List<string> { pointStatsByMatchAndNumbersKey.SubKey }, CachingConfigs.RedisConnectionForApp);
                if (!vals.TryGetValue(pointStatsByMatchAndNumbersKey.SubKey, out string currentPoint) || string.IsNullOrEmpty(currentPoint) || !decimal.TryParse(currentPoint, out decimal oldPoints)) oldPoints = 0m;

                var newPoints = oldPoints + item.Value;
                await _cacheService.HashSetFieldsAsync(pointStatsByMatchAndNumbersKey.MainKey, new Dictionary<string, string>
                {
                    { pointStatsByMatchAndNumbersKey.SubKey, newPoints.ToString() }
                }, pointStatsByMatchAndNumbersKey.TimeSpan == TimeSpan.Zero ? null : pointStatsByMatchAndNumbersKey.TimeSpan, CachingConfigs.RedisConnectionForApp);
            }

            //  Payout
            foreach (var item in payoutByNumbers)
            {
                var payoutStatsByMatchAndNumbersKey = matchId.GetPayoutStatsKeyByMatchBetKindNumber(betKindId, item.Key);
                var vals = await _cacheService.HashGetFieldsAsync(payoutStatsByMatchAndNumbersKey.MainKey, new List<string> { payoutStatsByMatchAndNumbersKey.SubKey }, CachingConfigs.RedisConnectionForApp);
                if (!vals.TryGetValue(payoutStatsByMatchAndNumbersKey.SubKey, out string currentPoint) || string.IsNullOrEmpty(currentPoint) || !decimal.TryParse(currentPoint, out decimal oldPayout)) oldPayout = 0m;

                var newPayout = oldPayout + item.Value;
                await _cacheService.HashSetFieldsAsync(payoutStatsByMatchAndNumbersKey.MainKey, new Dictionary<string, string>
                {
                    { payoutStatsByMatchAndNumbersKey.SubKey, newPayout.ToString() }
                }, payoutStatsByMatchAndNumbersKey.TimeSpan == TimeSpan.Zero ? null : payoutStatsByMatchAndNumbersKey.TimeSpan, CachingConfigs.RedisConnectionForApp);
            }
        }

        public async Task<(decimal, bool)> GetGivenCredit(long playerId)
        {
            var givenCreditKey = playerId.GetPlayerGivenCredit();
            var givenCredit = await _cacheService.HashGetFieldsAsync(givenCreditKey.MainKey, new List<string> { givenCreditKey.SubKey }, CachingConfigs.RedisConnectionForApp);
            if (givenCredit.Count == 0) return (await GetGivenCreditInDb(playerId), true);
            var givenCreditValue = givenCredit.FirstOrDefault().Value;
            if (string.IsNullOrEmpty(givenCreditValue) || !decimal.TryParse(givenCreditValue, out decimal val)) return (await GetGivenCreditInDb(playerId), true);
            return (val, false);
        }

        private async Task<decimal> GetGivenCreditInDb(long playerId)
        {
            var playerRepository = LotteryUow.GetRepository<IPlayerRepository>();
            var player = await playerRepository.FindByIdAsync(playerId);
            return player != null ? player.Credit : 0m;
        }

        public async Task<Dictionary<int, decimal>> GetMatchPlayerOddsByBetKindAndNumbers(long playerId, decimal defaultOddsValue, long matchId, int betKindId, List<int> numbers)
        {
            var odds = new Dictionary<int, decimal>();
            for (var i = 0; i < 100; i++) odds[i] = defaultOddsValue;

            foreach (var number in numbers)
            {
                var key = playerId.GetPlayerOddsByMatchBetKindAndNumber(matchId, betKindId, number);
                var data = await _cacheService.HashGetFieldsAsync(key.MainKey, new List<string> { key.SubKey }, CachingConfigs.RedisConnectionForApp);
                if (data.Count == 0) continue;
                if (!data.TryGetValue(key.SubKey, out string sOddValue) || string.IsNullOrEmpty(sOddValue)) continue;
                if (!decimal.TryParse(sOddValue, out decimal oddValue)) continue;

                odds[number] = oddValue;
            }

            return odds;
        }

        public async Task<Dictionary<int, decimal>> GetMatchPlayerMixedOddsByBetKind(long playerId, long matchId, int originBetKindId, Dictionary<int, BetSettingModel> subBetKinds)
        {
            var odds = new Dictionary<int, decimal>();
            foreach (var item in subBetKinds)
            {
                if (item.Value != null) odds[item.Key] = item.Value.OddsValue;

                var key = playerId.GetMixedPlayerOddsByMatch(matchId, originBetKindId, item.Key);
                var data = await _cacheService.HashGetFieldsAsync(key.MainKey, new List<string> { key.SubKey }, CachingConfigs.RedisConnectionForApp);
                if (data.Count == 0) continue;
                if (!data.TryGetValue(key.SubKey, out string sOddValue) || string.IsNullOrEmpty(sOddValue)) continue;
                if (!decimal.TryParse(sOddValue, out decimal oddValue)) continue;
                odds[item.Key] = oddValue;
            }
            return odds;
        }

        public async Task<AgentMixedOddsModel> GetAgentMixedOdds(int originBetKindId, List<int> subBetKindIds, long supermasterId, long masterId, long agentId)
        {
            var agentOdds = await _oddService.GetAgentOddsBy(subBetKindIds, new List<long> { supermasterId, masterId, agentId });
            var dictAgentOdds = CalculateAgentMixedOdds(subBetKindIds, agentId, agentOdds);
            var dictMasterOdds = CalculateAgentMixedOdds(subBetKindIds, masterId, agentOdds);
            var dictSupermasterOdds = CalculateAgentMixedOdds(subBetKindIds, supermasterId, agentOdds);
            return new AgentMixedOddsModel
            {
                CompanyOdds = await GetCompanyMixedOdds(subBetKindIds),
                SupermasterOdds = dictSupermasterOdds,
                MasterOdds = dictMasterOdds,
                AgentOdds = dictAgentOdds
            };
        }

        private Dictionary<int, decimal> CalculateAgentMixedOdds(List<int> subBetKindIds, long agentId, List<OddsModel> agentOdds)
        {
            var dataOdds = new Dictionary<int, decimal>();
            foreach (var betKindId in subBetKindIds)
            {
                var oddValue = agentOdds.FirstOrDefault(f => f.AgentId == agentId && f.BetKindId == betKindId);
                if (oddValue == null) continue;
                dataOdds[betKindId] = oddValue.Buy;
            }
            return dataOdds;
        }

        public async Task<AgentOddsForProcessModel> GetAgentOdds(int betKindId, long supermasterId, long masterId, long agentId)
        {
            var agentOdds = await _oddService.GetAgentOddsBy(betKindId, new List<long> { supermasterId, masterId, agentId });
            var dictAgentOdds = CalculateAgentOdds(agentId, agentOdds);
            var dictMasterOdds = CalculateAgentOdds(masterId, agentOdds);
            var dictSupermasterOdds = CalculateAgentOdds(supermasterId, agentOdds);
            return new AgentOddsForProcessModel
            {
                CompanyOdds = await GetCompanyOdds(betKindId),
                SupermasterOdds = dictSupermasterOdds,
                MasterOdds = dictMasterOdds,
                AgentOdds = dictAgentOdds
            };
        }

        private async Task<Dictionary<int, decimal>> GetCompanyMixedOdds(List<int> subBetKindIds)
        {
            var defaultOddsRepository = _inMemoryUnitOfWork.GetRepository<IDefaultOddsInMemoryRepository>();
            var defaultOddsOfBetKind = defaultOddsRepository.FindBy(f => subBetKindIds.Contains(f.BetKindId)).ToList();
            if (defaultOddsOfBetKind.Count == 0) defaultOddsOfBetKind = await _oddService.GetDefaultOddsByBetKind(subBetKindIds);
            var companyOdds = new Dictionary<int, decimal>();
            foreach (var betKindId in subBetKindIds)
            {
                var oddValue = defaultOddsOfBetKind.FirstOrDefault(f => f.BetKindId == betKindId);
                if (oddValue == null) continue;
                companyOdds[betKindId] = oddValue.Buy;
            }
            return companyOdds;
        }

        private async Task<Dictionary<int, decimal>> GetCompanyOdds(int betKindId)
        {
            var defaultOddsRepository = _inMemoryUnitOfWork.GetRepository<IDefaultOddsInMemoryRepository>();
            var defaultOddsOfBetKind = defaultOddsRepository.FindBy(f => f.BetKindId == betKindId).FirstOrDefault();
            if (defaultOddsOfBetKind == null) defaultOddsOfBetKind = (await _oddService.GetDefaultOddsByBetKind(new List<int> { betKindId })).FirstOrDefault();
            if (defaultOddsOfBetKind == null) return new Dictionary<int, decimal>();
            var companyOdds = new Dictionary<int, decimal>();
            for (var i = 0; i < 100; i++) companyOdds[i] = defaultOddsOfBetKind.Buy;
            return companyOdds;
        }

        private Dictionary<int, decimal> CalculateAgentOdds(long agentId, List<OddsModel> agentOdds)
        {
            var dataOdds = new Dictionary<int, decimal>();
            var oddValue = agentOdds.FirstOrDefault(f => f.AgentId == agentId);
            for (var i = 0; i < 100; i++) dataOdds[i] = oddValue.Buy;
            return dataOdds;
        }

        public async Task<PlayerOutsModel> GetOuts(long playerId, long matchId, List<int> numbers)
        {
            return new PlayerOutsModel
            {
                OutsByMatch = await GetOutsByMatch(playerId, matchId),
                PointsByMatchAndNumbers = await GetPointsByMatchAndNumbers(playerId, matchId, numbers)
            };
        }

        private async Task<decimal> GetOutsByMatch(long playerId, long matchId)
        {
            var outsKey = playerId.GetPlayerOutsByMatch(matchId);
            var outs = await _cacheService.HashGetFieldsAsync(outsKey.MainKey, new List<string> { outsKey.SubKey }, CachingConfigs.RedisConnectionForApp);
            if (outs.Count == 0) return 0m;
            var outsValue = outs.FirstOrDefault().Value;
            if (string.IsNullOrEmpty(outsValue) || !decimal.TryParse(outsValue, out decimal val)) return 0m;
            return val;
        }

        private async Task<Dictionary<int, decimal>> GetPointsByMatchAndNumbers(long playerId, long matchId, List<int> numbers)
        {
            var d = new Dictionary<int, decimal>();
            foreach (var number in numbers)
            {
                var pointsByMatchAndNumberKey = playerId.GetPlayerPointsByMatchAndNumber(matchId, number);
                var outs = await _cacheService.HashGetFieldsAsync(pointsByMatchAndNumberKey.MainKey, new List<string> { pointsByMatchAndNumberKey.SubKey }, CachingConfigs.RedisConnectionForApp);
                if (outs.Count == 0) continue;
                var outsValue = outs.FirstOrDefault().Value;
                if (string.IsNullOrEmpty(outsValue) || !decimal.TryParse(outsValue, out decimal val)) continue;
                d[number] = val;
            }
            return d;
        }

        public async Task<PlayerOutsModel> GetOuts(long playerId, long matchId)
        {
            return new PlayerOutsModel
            {
                OutsByMatch = await GetOutsByMatch(playerId, matchId)
            };
        }
    }
}
