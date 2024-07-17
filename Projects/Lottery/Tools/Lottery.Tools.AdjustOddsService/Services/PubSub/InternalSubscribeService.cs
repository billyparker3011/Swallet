using HnMicro.Module.Caching.ByRedis.Services;
using HnMicro.Modules.InMemory.UnitOfWorks;
using Lottery.Core.Configs;
using Lottery.Core.InMemory.Setting;
using Lottery.Core.Models.Match;
using Lottery.Core.Models.Payouts;
using Lottery.Core.Models.Setting;
using Lottery.Tools.AdjustOddsService.InMemory.Payouts;
using Lottery.Tools.AdjustOddsService.Services.AdjustOdds;
using Lottery.Tools.AdjustOddsService.Services.AdjustOdds.Commands;

namespace Lottery.Tools.AdjustOddsService.Services.PubSub
{
    public class InternalSubscribeService : IInternalSubscribeService
    {
        private readonly IRedisCacheService _redisCacheService;
        private readonly IInMemoryUnitOfWork _inMemoryUnitOfWork;
        private readonly IOddsAdjustmentService _oddsAdjustmentService;

        public InternalSubscribeService(IRedisCacheService redisCacheService, IInMemoryUnitOfWork inMemoryUnitOfWork, IOddsAdjustmentService oddsAdjustmentService)
        {
            _redisCacheService = redisCacheService;
            _inMemoryUnitOfWork = inMemoryUnitOfWork;
            _oddsAdjustmentService = oddsAdjustmentService;
        }

        public void Start()
        {
            SubscribeSettings();
            SubscribeMixedCompanyPayouts();
            SubscribeCompanyPayouts();
            SubscribeCompletedMatch();
        }

        private void SubscribeCompletedMatch()
        {
            _redisCacheService.Subscribe(SubscribeCommonConfigs.CompletedMatchChannel, (channel, message) =>
            {
                ProcessCompletedMatch(message);
            }, CachingConfigs.RedisConnectionForApp);
        }

        private void ProcessCompletedMatch(string message)
        {
            var completedMatch = Newtonsoft.Json.JsonConvert.DeserializeObject<CompletedMatchModel>(message);
            if (completedMatch == null || completedMatch.MatchId <= 0L) return;
            _oddsAdjustmentService.Enqueue(new DeleteMatchCommand
            {
                MatchId = completedMatch.MatchId
            });
        }

        private void SubscribeMixedCompanyPayouts()
        {
            _redisCacheService.Subscribe(SubscribeCommonConfigs.MixedCompanyPayoutChannel, (channel, message) =>
            {
                ProcessMixedCompanyPayout(message);
            }, CachingConfigs.RedisConnectionForApp);
        }

        private void SubscribeCompanyPayouts()
        {
            _redisCacheService.Subscribe(SubscribeCommonConfigs.CompanyPayoutChannel, (channel, message) =>
            {
                ProcessCompanyPayout(message);
            }, CachingConfigs.RedisConnectionForApp);
        }

        private void SubscribeSettings()
        {
            _redisCacheService.Subscribe(SubscribeCommonConfigs.PublishSettingChannel, (channel, message) =>
            {
                ProcessSetting(message);
            }, CachingConfigs.RedisConnectionForApp);
        }

        private void ProcessMixedCompanyPayout(string message)
        {
            var companyPayouts = Newtonsoft.Json.JsonConvert.DeserializeObject<MixedCompanyPayoutModel>(message);
            if (companyPayouts.Payouts.Count == 0) return;

            var dict = new Dictionary<int, List<string>>();

            var payoutByMixedAndNumberInMemoryRepository = _inMemoryUnitOfWork.GetRepository<IPayoutByMixedAndNumberInMemoryRepository>();
            foreach (var item in companyPayouts.Payouts)
            {
                foreach (var pair in item.Value)
                {
                    List<string> listPairs;
                    if (!dict.TryGetValue(item.Key, out listPairs))
                    {
                        listPairs = new List<string>();
                        dict[item.Key] = listPairs;
                    }

                    if (!listPairs.Contains(pair.Key)) listPairs.Add(pair.Key);

                    payoutByMixedAndNumberInMemoryRepository.Add(new Models.Payouts.PayoutByMixedAndNumberModel
                    {
                        MatchId = companyPayouts.MatchId,
                        BetKindId = item.Key,
                        PairNumber = pair.Key,
                        Payout = pair.Value
                    });
                }
            }

            _oddsAdjustmentService.Enqueue(new ChangePayoutOfPairNumbersCommand
            {
                MatchId = companyPayouts.MatchId,
                PairNumbers = dict
            });
        }

        private void ProcessCompanyPayout(string message)
        {
            var companyPayouts = Newtonsoft.Json.JsonConvert.DeserializeObject<CompanyPayoutModel>(message);
            if (companyPayouts.Payouts.Count == 0) return;
            var payoutByBetKindAndNumberInMemoryRepository = _inMemoryUnitOfWork.GetRepository<IPayoutByBetKindAndNumberInMemoryRepository>();
            foreach (var item in companyPayouts.Payouts)
            {
                payoutByBetKindAndNumberInMemoryRepository.Add(new Models.Payouts.PayoutByBetKindAndNumberModel
                {
                    MatchId = companyPayouts.MatchId,
                    BetKindId = companyPayouts.BetKindId,
                    Number = item.Key,
                    Payout = item.Value
                });
            }

            _oddsAdjustmentService.Enqueue(new ChangePayoutOfNumbersCommand
            {
                MatchId = companyPayouts.MatchId,
                BetKindId = companyPayouts.BetKindId,
                Numbers = companyPayouts.Payouts.Select(f => f.Key).ToList()
            });
        }

        private void ProcessSetting(string message)
        {
            var setting = Newtonsoft.Json.JsonConvert.DeserializeObject<SettingModel>(message);
            if (setting == null || setting.Id == 0) return;
            var inmemorySettingRepository = _inMemoryUnitOfWork.GetRepository<ISettingInMemoryRepository>();
            inmemorySettingRepository.Update(setting);
        }
    }
}
