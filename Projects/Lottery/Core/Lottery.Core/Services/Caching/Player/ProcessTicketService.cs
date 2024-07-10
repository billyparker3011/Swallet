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

        public async Task UpdateOutsByMatchCache(Dictionary<long, Dictionary<long, decimal>> downOuts)
        {
            foreach (var item in downOuts)
            {
                var playerId = item.Key;
                foreach (var subItem in item.Value)
                {
                    var outsKey = playerId.GetPlayerOutsByMatch(subItem.Key);
                    var currentOuts = await _cacheService.HashGetFieldsAsync(outsKey.MainKey, new List<string> { outsKey.SubKey }, CachingConfigs.RedisConnectionForApp);
                    if (currentOuts == null || currentOuts.Count == 0) continue;
                    if (!currentOuts.TryGetValue(outsKey.SubKey, out string sOuts)) continue;
                    if (!decimal.TryParse(sOuts, out decimal outs)) continue;
                    if (outs < subItem.Value) continue;

                    currentOuts[outsKey.SubKey] = (outs - subItem.Value).ToString();
                    await _cacheService.HashSetFieldsAsync(outsKey.MainKey, currentOuts, outsKey.TimeSpan, CachingConfigs.RedisConnectionForApp);
                }
            }
        }

        public async Task BuildPointsByMatchAndNumbersCache(long playerId, long matchId, Dictionary<int, decimal> pointsByMatchAndNumbers, Dictionary<int, decimal> pointByNumbers)
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

        public async Task UpdatePointsByMatchAndNumbersCache(Dictionary<long, Dictionary<long, Dictionary<int, decimal>>> points)
        {
            //  Key = PlayerId => Key = MatchId => Key = Number
            foreach (var itemPoint in points)
            {
                foreach (var itemMatch in itemPoint.Value)
                {
                    foreach (var itemNumber in itemMatch.Value)
                    {
                        var pointsByMatchAndNumberKey = itemMatch.Key.GetPlayerPointsByMatchAndNumber(itemMatch.Key, itemNumber.Key);
                        var currentPoints = await _cacheService.HashGetFieldsAsync(pointsByMatchAndNumberKey.MainKey, new List<string> { pointsByMatchAndNumberKey.SubKey }, CachingConfigs.RedisConnectionForApp);
                        if (currentPoints == null || currentPoints.Count == 0) continue;
                        if (!currentPoints.TryGetValue(pointsByMatchAndNumberKey.SubKey, out string sPoints)) continue;
                        if (!decimal.TryParse(sPoints, out decimal valPoints)) continue;
                        if (valPoints < itemNumber.Value) continue;

                        currentPoints[pointsByMatchAndNumberKey.SubKey] = (valPoints - itemNumber.Value).ToString();
                        await _cacheService.HashSetFieldsAsync(pointsByMatchAndNumberKey.MainKey, currentPoints, pointsByMatchAndNumberKey.TimeSpan, CachingConfigs.RedisConnectionForApp);
                    }
                }
            }
        }

        public async Task BuildStatsByMatchBetKindAndNumbers(long matchId, int betKindId, Dictionary<int, decimal> pointByNumbers, Dictionary<int, decimal> payoutByNumbers, Dictionary<int, decimal> companyPayoutByNumbers)
        {
            //  Super
            //      Point
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

            //      Payout
            foreach (var item in payoutByNumbers)
            {
                var payoutStatsByMatchAndNumbersKey = matchId.GetPayoutStatsKeyByMatchBetKindNumber(betKindId, item.Key);
                var vals = await _cacheService.HashGetFieldsAsync(payoutStatsByMatchAndNumbersKey.MainKey, new List<string> { payoutStatsByMatchAndNumbersKey.SubKey }, CachingConfigs.RedisConnectionForApp);
                if (!vals.TryGetValue(payoutStatsByMatchAndNumbersKey.SubKey, out string currentPayout) || string.IsNullOrEmpty(currentPayout) || !decimal.TryParse(currentPayout, out decimal oldPayout)) oldPayout = 0m;

                var newPayout = oldPayout + item.Value;
                await _cacheService.HashSetFieldsAsync(payoutStatsByMatchAndNumbersKey.MainKey, new Dictionary<string, string>
                {
                    { payoutStatsByMatchAndNumbersKey.SubKey, newPayout.ToString() }
                }, payoutStatsByMatchAndNumbersKey.TimeSpan == TimeSpan.Zero ? null : payoutStatsByMatchAndNumbersKey.TimeSpan, CachingConfigs.RedisConnectionForApp);
            }

            //  Company
            foreach (var item in companyPayoutByNumbers)
            {
                var companyPayoutStatsByMatchAndNumbersKey = matchId.GetCompanyPayoutStatsKeyByMatchBetKindNumber(betKindId, item.Key);
                var vals = await _cacheService.HashGetFieldsAsync(companyPayoutStatsByMatchAndNumbersKey.MainKey, new List<string> { companyPayoutStatsByMatchAndNumbersKey.SubKey }, CachingConfigs.RedisConnectionForApp);
                if (!vals.TryGetValue(companyPayoutStatsByMatchAndNumbersKey.SubKey, out string currentCompanyPayout) || string.IsNullOrEmpty(currentCompanyPayout) || !decimal.TryParse(currentCompanyPayout, out decimal oldPayout)) oldPayout = 0m;

                var newPayout = oldPayout + item.Value;
                await _cacheService.HashSetFieldsAsync(companyPayoutStatsByMatchAndNumbersKey.MainKey, new Dictionary<string, string>
                {
                    { companyPayoutStatsByMatchAndNumbersKey.SubKey, newPayout.ToString() }
                }, companyPayoutStatsByMatchAndNumbersKey.TimeSpan == TimeSpan.Zero ? null : companyPayoutStatsByMatchAndNumbersKey.TimeSpan, CachingConfigs.RedisConnectionForApp);
            }
        }

        public async Task BuildMixedStatsByMatch(long matchId, Dictionary<int, Dictionary<string, decimal>> pointByPair, Dictionary<int, Dictionary<string, decimal>> payoutByPair, Dictionary<int, Dictionary<string, decimal>> realPayoutByPair)
        {
            foreach (var item in pointByPair)
            {
                var pointMixStatsByMatchKey = matchId.GetPointMixedStatsKeyByMatchBetKind(item.Key);
                var existed = await _cacheService.ExistsAsync(pointMixStatsByMatchKey.MainKey, CachingConfigs.RedisConnectionForApp);
                var i = 0;
                foreach (var subItem in item.Value)
                {
                    if (i == 0)
                    {
                        if (!existed)
                        {
                            await _cacheService.SortedSetAddAsync(pointMixStatsByMatchKey.MainKey, subItem.Key, subItem.Value, pointMixStatsByMatchKey.TimeSpan, CachingConfigs.RedisConnectionForApp);
                            existed = true;
                        }
                        else await _cacheService.SortedSetIncrementAsync(pointMixStatsByMatchKey.MainKey, subItem.Key, subItem.Value, CachingConfigs.RedisConnectionForApp);
                    }
                    await _cacheService.SortedSetIncrementAsync(pointMixStatsByMatchKey.MainKey, subItem.Key, subItem.Value, CachingConfigs.RedisConnectionForApp);
                    i++;
                }
            }

            foreach (var item in payoutByPair)
            {
                var payoutMixStatsByMatchKey = matchId.GetPayoutMixedStatsKeyByMatchBetKind(item.Key);
                var existed = await _cacheService.ExistsAsync(payoutMixStatsByMatchKey.MainKey, CachingConfigs.RedisConnectionForApp);
                var i = 0;
                foreach (var subItem in item.Value)
                {
                    if (i == 0)
                    {
                        if (!existed)
                        {
                            await _cacheService.SortedSetAddAsync(payoutMixStatsByMatchKey.MainKey, subItem.Key, subItem.Value, payoutMixStatsByMatchKey.TimeSpan, CachingConfigs.RedisConnectionForApp);
                            existed = true;
                        }
                        else await _cacheService.SortedSetIncrementAsync(payoutMixStatsByMatchKey.MainKey, subItem.Key, subItem.Value, CachingConfigs.RedisConnectionForApp);
                    }
                    await _cacheService.SortedSetIncrementAsync(payoutMixStatsByMatchKey.MainKey, subItem.Key, subItem.Value, CachingConfigs.RedisConnectionForApp);
                    i++;
                }
            }

            foreach (var item in realPayoutByPair)
            {
                var companyPayoutMixStatsByMatchKey = matchId.GetCompanyPayoutMixedStatsKeyByMatchBetKind(item.Key);
                var existed = await _cacheService.ExistsAsync(companyPayoutMixStatsByMatchKey.MainKey, CachingConfigs.RedisConnectionForApp);
                var i = 0;
                foreach (var subItem in item.Value)
                {
                    if (i == 0)
                    {
                        if (!existed)
                        {
                            await _cacheService.SortedSetAddAsync(companyPayoutMixStatsByMatchKey.MainKey, subItem.Key, subItem.Value, companyPayoutMixStatsByMatchKey.TimeSpan, CachingConfigs.RedisConnectionForApp);
                            existed = true;
                        }
                        else await _cacheService.SortedSetIncrementAsync(companyPayoutMixStatsByMatchKey.MainKey, subItem.Key, subItem.Value, CachingConfigs.RedisConnectionForApp);
                    }
                    await _cacheService.SortedSetIncrementAsync(companyPayoutMixStatsByMatchKey.MainKey, subItem.Key, subItem.Value, CachingConfigs.RedisConnectionForApp);
                    i++;
                }
            }
        }

        public async Task UpdateStatsByMatchBetKindAndNumbers(Dictionary<int, Dictionary<long, Dictionary<int, decimal>>> outs, Dictionary<int, Dictionary<long, Dictionary<int, decimal>>> points)
        {
            //  Outs
            foreach (var itemBetKind in outs)
            {
                foreach (var itemMatch in itemBetKind.Value)
                {
                    foreach (var itemNumber in itemMatch.Value)
                    {
                        var outsKey = itemMatch.Key.GetPayoutStatsKeyByMatchBetKindNumber(itemBetKind.Key, itemNumber.Key);
                        var outsVals = await _cacheService.HashGetFieldsAsync(outsKey.MainKey, new List<string> { outsKey.SubKey }, CachingConfigs.RedisConnectionForApp);
                        if (outsVals == null || outsVals.Count == 0) continue;
                        if (!outsVals.TryGetValue(outsKey.SubKey, out string sCurrentOuts)) continue;
                        if (!decimal.TryParse(sCurrentOuts, out decimal currentOuts)) continue;
                        if (currentOuts < itemNumber.Value) continue;

                        outsVals[outsKey.SubKey] = (currentOuts - itemNumber.Value).ToString();
                        await _cacheService.HashSetFieldsAsync(outsKey.MainKey, outsVals, outsKey.TimeSpan, CachingConfigs.RedisConnectionForApp);
                    }
                }
            }
            //  Points
            foreach (var itemBetKind in points)
            {
                foreach (var itemMatch in itemBetKind.Value)
                {
                    foreach (var itemNumber in itemMatch.Value)
                    {
                        var pointsKey = itemMatch.Key.GetPointStatsKeyByMatchBetKindNumber(itemBetKind.Key, itemNumber.Key);

                        var pointsVals = await _cacheService.HashGetFieldsAsync(pointsKey.MainKey, new List<string> { pointsKey.SubKey }, CachingConfigs.RedisConnectionForApp);
                        if (pointsVals == null || pointsVals.Count == 0) continue;
                        if (!pointsVals.TryGetValue(pointsKey.SubKey, out string sCurrentPoints)) continue;
                        if (!decimal.TryParse(sCurrentPoints, out decimal currentPoints)) continue;
                        if (currentPoints < itemNumber.Value) continue;

                        pointsVals[pointsKey.SubKey] = (currentPoints - itemNumber.Value).ToString();
                        await _cacheService.HashSetFieldsAsync(pointsKey.MainKey, pointsVals, pointsKey.TimeSpan, CachingConfigs.RedisConnectionForApp);
                    }
                }
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

        public async Task<Dictionary<int, decimal>> GetMatchPlayerOddsByBetKindAndNumbers(long playerId, decimal defaultOddsValue, long matchId, int betKindId, List<int> numbers, int noOfNumbers = 100)
        {
            var odds = new Dictionary<int, decimal>();
            for (var i = 0; i < noOfNumbers; i++) odds[i] = defaultOddsValue;

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

        public async Task<AgentOddsForProcessModel> GetAgentOdds(int betKindId, long supermasterId, long masterId, long agentId, int noOfNumbers = 100)
        {
            var agentOdds = await _oddService.GetAgentOddsBy(betKindId, new List<long> { supermasterId, masterId, agentId });
            var dictAgentOdds = CalculateAgentOdds(agentId, agentOdds, noOfNumbers);
            var dictMasterOdds = CalculateAgentOdds(masterId, agentOdds, noOfNumbers);
            var dictSupermasterOdds = CalculateAgentOdds(supermasterId, agentOdds, noOfNumbers);
            return new AgentOddsForProcessModel
            {
                CompanyOdds = await GetCompanyOdds(betKindId, noOfNumbers),
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

        private async Task<Dictionary<int, decimal>> GetCompanyOdds(int betKindId, int noOfNumbers = 100)
        {
            var defaultOddsRepository = _inMemoryUnitOfWork.GetRepository<IDefaultOddsInMemoryRepository>();
            var defaultOddsOfBetKind = defaultOddsRepository.FindBy(f => f.BetKindId == betKindId).FirstOrDefault();
            if (defaultOddsOfBetKind == null) defaultOddsOfBetKind = (await _oddService.GetDefaultOddsByBetKind(new List<int> { betKindId })).FirstOrDefault();
            if (defaultOddsOfBetKind == null) return new Dictionary<int, decimal>();
            var companyOdds = new Dictionary<int, decimal>();
            for (var i = 0; i < noOfNumbers; i++) companyOdds[i] = defaultOddsOfBetKind.Buy;
            return companyOdds;
        }

        private Dictionary<int, decimal> CalculateAgentOdds(long agentId, List<OddsModel> agentOdds, int noOfNumbers = 100)
        {
            var dataOdds = new Dictionary<int, decimal>();
            var oddValue = agentOdds.FirstOrDefault(f => f.AgentId == agentId);
            for (var i = 0; i < noOfNumbers; i++) dataOdds[i] = oddValue.Buy;
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
