using HnMicro.Modules.InMemory.UnitOfWorks;
using Lottery.Core.InMemory.Setting;
using Lottery.Core.Models.Setting.BetKind;
using Lottery.Core.Services.Odds;
using Lottery.Core.Services.Setting;
using Lottery.Tools.AdjustOddsService.InMemory.Payouts;
using Lottery.Tools.AdjustOddsService.Services.AdjustOdds.Commands;

namespace Lottery.Tools.AdjustOddsService.Services.AdjustOdds.Handlers
{
    public class ChangePayoutOfNumbersCommandHandler : AdjustOddsCommandHandler
    {
        public ChangePayoutOfNumbersCommandHandler(IServiceProvider serviceProvider) : base(serviceProvider)
        {
        }

        public override string Command { get; set; } = nameof(ChangePayoutOfNumbersCommand);

        public override void Handler(AdjustOddsCommand command)
        {
            var deviredAdjustCommand = (ChangePayoutOfNumbersCommand)command;

            using var scope = ServiceProvider.CreateScope();
            var inMemoryUnitOfWork = scope.ServiceProvider.GetService<IInMemoryUnitOfWork>();

            var payoutByBetKindAndNumberInMemoryRepository = inMemoryUnitOfWork.GetRepository<IPayoutByBetKindAndNumberInMemoryRepository>();

            var inMemorySettingRepository = inMemoryUnitOfWork.GetRepository<ISettingInMemoryRepository>();

            var balanceTableSettingService = scope.ServiceProvider.GetService<IBalanceTableSettingService>();
            var balanceTableKey = balanceTableSettingService.CreateBalanceTableKey(deviredAdjustCommand.BetKindId);

            var setting = inMemorySettingRepository.FindByKey(balanceTableKey);
            if (setting == null || string.IsNullOrEmpty(setting.ValueSetting)) return;

            var settingVal = Newtonsoft.Json.JsonConvert.DeserializeObject<BalanceTableModel>(setting.ValueSetting);
            if (settingVal == null) return;

            var dictRate = new Dictionary<int, decimal>();
            foreach (var item in deviredAdjustCommand.Numbers)
            {
                var payoutByBetKindAndNumber = payoutByBetKindAndNumberInMemoryRepository.FindByBetKindNumber(deviredAdjustCommand.MatchId, deviredAdjustCommand.BetKindId, item);
                if (payoutByBetKindAndNumber == null) continue;

                var rate = settingVal.ByNumbers.Numbers.Count > 0 && settingVal.ByNumbers.Numbers.Contains(item)
                    ? settingVal.ByNumbers.RateValues.FirstOrDefault(f => f.From <= payoutByBetKindAndNumber.Payout && payoutByBetKindAndNumber.Payout < f.To && !f.Applied)
                    : settingVal.ForCommon.RateValues.FirstOrDefault(f => f.From <= payoutByBetKindAndNumber.Payout && payoutByBetKindAndNumber.Payout < f.To && !f.Applied);
                if (rate == null) continue;

                rate.Applied = true;
                setting.ValueSetting = Newtonsoft.Json.JsonConvert.SerializeObject(settingVal);
                dictRate[item] = rate.RateValue;
            }

            if (dictRate.Count == 0) return;

            var processOddsService = scope.ServiceProvider.GetService<IProcessOddsService>();
            processOddsService.UpdateRateOfOddsValue(deviredAdjustCommand.MatchId, deviredAdjustCommand.BetKindId, dictRate);
        }
    }
}
