using HnMicro.Core.Helpers;
using HnMicro.Modules.InMemory.UnitOfWorks;
using Lottery.Core.Enums;
using Lottery.Core.InMemory.Setting;
using Lottery.Core.Models.Setting.BetKind;
using Lottery.Core.Services.Odds;
using Lottery.Core.Services.Setting;
using Lottery.Tools.AdjustOddsService.InMemory.Payouts;
using Lottery.Tools.AdjustOddsService.Services.AdjustOdds.Commands;

namespace Lottery.Tools.AdjustOddsService.Services.AdjustOdds.Handlers
{
    public class ChangePayoutOfPairNumbersCommandHandler : AdjustOddsCommandHandler
    {
        public ChangePayoutOfPairNumbersCommandHandler(IServiceProvider serviceProvider) : base(serviceProvider)
        {
        }

        public override string Command { get; set; } = nameof(ChangePayoutOfPairNumbersCommand);

        public override void Handler(AdjustOddsCommand command)
        {
            var deviredAdjustCommand = (ChangePayoutOfPairNumbersCommand)command;

            using var scope = ServiceProvider.CreateScope();
            var inMemoryUnitOfWork = scope.ServiceProvider.GetService<IInMemoryUnitOfWork>();

            var payoutByMixedAndNumberInMemoryRepository = inMemoryUnitOfWork.GetRepository<IPayoutByMixedAndNumberInMemoryRepository>();
            var settingInMemoryRepository = inMemoryUnitOfWork.GetRepository<ISettingInMemoryRepository>();

            var balanceTableSettingService = scope.ServiceProvider.GetService<IBalanceTableSettingService>();
            var balanceTableKey = balanceTableSettingService.CreateBalanceTableKey(BetKind.FirstNorthern_Northern_LoXien.ToInt());

            var setting = settingInMemoryRepository.FindByKey(balanceTableKey);
            if (setting == null || string.IsNullOrEmpty(setting.ValueSetting)) return;

            var settingVal = Newtonsoft.Json.JsonConvert.DeserializeObject<BalanceTableModel>(setting.ValueSetting);
            if (settingVal == null) return;

            var listBetKind = new Dictionary<int, decimal>();
            //  By numbers
            if (settingVal.ByNumbers.Numbers.Count > 0)
            {
                foreach (var item in deviredAdjustCommand.PairNumbers)
                {
                    var betKindId = item.Key;
                    var pairs = item.Value; //  [ "00, 01", "02, 04", "05, 06" ]
                    var checkPairs = true;
                    decimal? rateValue = null;
                    foreach (var itemPair in pairs)
                    {
                        var payoutByPair = payoutByMixedAndNumberInMemoryRepository.FindByBetKindNumber(deviredAdjustCommand.MatchId, betKindId, itemPair);
                        if (payoutByPair == null) continue;

                        var rate = settingVal.ByNumbers.RateValues.FirstOrDefault(f => balanceTableSettingService.GetRealValue(f.From) <= payoutByPair.Payout && payoutByPair.Payout < balanceTableSettingService.GetRealValue(f.To));
                        if (rate == null) continue;
                        if (rate.AppliedNumbers.Any(f => f < 0)) continue;  //  I use this value < 0 because we cannot use no of numbers. Maybe no of mixed tickets are equal no of number.
                        if (rate.PairNumbers.Any(f => f.Equals(itemPair))) continue;

                        var arrItemPair = itemPair.Split(new[] { "," }, StringSplitOptions.RemoveEmptyEntries);
                        var isContains = true;
                        foreach (var itemInArr in arrItemPair)
                        {
                            if (!int.TryParse(itemInArr.Trim(), out int number)) continue;
                            if (settingVal.ByNumbers.Numbers.Contains(number)) continue;

                            isContains = false;
                            break;
                        }
                        if (!isContains)
                        {
                            rateValue = rate.RateValue;
                            rate.PairNumbers.Add(itemPair);
                        }
                        checkPairs &= isContains;
                    }
                    if (!checkPairs || !rateValue.HasValue) continue;
                    listBetKind[betKindId] = rateValue.Value;
                }
            }

            //  By common
            foreach (var item in deviredAdjustCommand.PairNumbers)
            {
                var betKindId = item.Key;
                if (listBetKind.ContainsKey(betKindId)) continue;

                var pairs = item.Value; //  [ "00, 01", "02, 04", "05, 06" ]
                var checkPairs = true;
                decimal? rateValue = null;
                foreach (var itemPair in pairs)
                {
                    var payoutByPair = payoutByMixedAndNumberInMemoryRepository.FindByBetKindNumber(deviredAdjustCommand.MatchId, betKindId, itemPair);
                    if (payoutByPair == null) continue;

                    var rate = settingVal.ForCommon.RateValues.FirstOrDefault(f => balanceTableSettingService.GetRealValue(f.From) <= payoutByPair.Payout && payoutByPair.Payout < balanceTableSettingService.GetRealValue(f.To));
                    if (rate == null) continue;
                    if (rate.AppliedNumbers.Any(f => f < 0)) continue;  //  I use this value < 0 because we cannot use no of numbers. Maybe no of mixed tickets are equal no of number.
                    if (rate.PairNumbers.Any(f => f.Equals(itemPair))) continue;

                    var arrItemPair = itemPair.Split(new[] { "," }, StringSplitOptions.RemoveEmptyEntries);
                    var isContains = true;
                    foreach (var itemInArr in arrItemPair)
                    {
                        if (!int.TryParse(itemInArr.Trim(), out int number)) continue;
                        if (settingVal.ByNumbers.Numbers.Contains(number)) continue;

                        isContains = false;
                        break;
                    }
                    if (!isContains)
                    {
                        rateValue = rate.RateValue;
                        rate.PairNumbers.Add(itemPair);
                    }
                    checkPairs &= isContains;
                }
                if (!checkPairs || !rateValue.HasValue) continue;
                listBetKind[betKindId] = rateValue.Value;
            }

            if (listBetKind.Count == 0) return;

            UpdateRateOfOddsValue(scope.ServiceProvider, deviredAdjustCommand.MatchId, listBetKind);
            balanceTableSettingService.UpdateSetting(setting);
        }

        private void UpdateRateOfOddsValue(IServiceProvider serviceProvider, long matchId, Dictionary<int, decimal> rateByBetKind)
        {
            Task.Run(async () =>
            {
                var processOddsService = serviceProvider.GetService<IProcessOddsService>();
                await processOddsService.UpdateMixedRateOfOddsValue(matchId, rateByBetKind);
            });
        }
    }
}
