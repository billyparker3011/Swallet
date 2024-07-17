using HnMicro.Core.Helpers;
using HnMicro.Modules.InMemory.UnitOfWorks;
using Lottery.Core.Enums;
using Lottery.Core.InMemory.Setting;
using Lottery.Core.Models.Setting.BetKind;
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

            var listBetKind = new List<int>();

            if (settingVal.ByNumbers.Numbers.Count > 0)
            {
                foreach (var item in deviredAdjustCommand.PairNumbers)
                {
                    var betKindId = item.Key;
                    var pairs = item.Value; //  [ "00, 01", "02, 04", "05, 06" ]
                    var checkPairs = true;
                    foreach (var itemPair in pairs)
                    {
                        var payoutByPair = payoutByMixedAndNumberInMemoryRepository.FindByBetKindNumber(deviredAdjustCommand.MatchId, betKindId, itemPair);
                        if (payoutByPair == null) continue;

                        var rate = settingVal.ByNumbers.RateValues.FirstOrDefault(f => balanceTableSettingService.GetRealValue(f.From) <= payoutByPair.Payout && payoutByPair.Payout < balanceTableSettingService.GetRealValue(f.To));
                        if (rate == null) continue;
                        if (rate.Applied) continue;

                        var arrItemPair = itemPair.Split(new[] { "," }, StringSplitOptions.RemoveEmptyEntries);
                        var isContains = true;
                        foreach (var itemInArr in arrItemPair)
                        {
                            if (!int.TryParse(itemInArr.Trim(), out int number)) continue;
                            if (settingVal.ByNumbers.Numbers.Contains(number)) continue;

                            isContains = false;
                            break;
                        }
                        checkPairs &= isContains;
                    }
                    if (!checkPairs) continue;
                    listBetKind.Add(betKindId);
                }
            }

            //var dictRate = new Dictionary<int, decimal>();
            //foreach (var item in deviredAdjustCommand.Numbers)
            //{
            //    var payoutByBetKindAndNumber = payoutByBetKindAndNumberInMemoryRepository.FindByBetKindNumber(deviredAdjustCommand.MatchId, deviredAdjustCommand.BetKindId, item);
            //    if (payoutByBetKindAndNumber == null) continue;

            //    var rate = settingVal.ByNumbers.Numbers.Count > 0 && settingVal.ByNumbers.Numbers.Contains(item)
            //        ? settingVal.ByNumbers.RateValues.FirstOrDefault(f => balanceTableSettingService.GetRealValue(f.From) <= payoutByBetKindAndNumber.Payout && payoutByBetKindAndNumber.Payout < balanceTableSettingService.GetRealValue(f.To))
            //        : settingVal.ForCommon.RateValues.FirstOrDefault(f => balanceTableSettingService.GetRealValue(f.From) <= payoutByBetKindAndNumber.Payout && payoutByBetKindAndNumber.Payout < balanceTableSettingService.GetRealValue(f.To));
            //    if (rate == null) continue;
            //    if (rate.AppliedNumbers.Contains(item)) continue;

            //    rate.AppliedNumbers.Add(item);
            //    setting.ValueSetting = Newtonsoft.Json.JsonConvert.SerializeObject(settingVal);
            //    dictRate[item] = rate.RateValue;
            //}

            //if (dictRate.Count == 0) return;

            //UpdateRateOfOddsValue(scope.ServiceProvider, deviredAdjustCommand.MatchId, deviredAdjustCommand.BetKindId, dictRate);
            //balanceTableSettingService.UpdateSetting(setting);

            //if (dictRate.Count == 0) return;

            //UpdateRateOfOddsValue(scope.ServiceProvider, deviredAdjustCommand.MatchId, deviredAdjustCommand.BetKindId, dictRate);
            //balanceTableSettingService.UpdateSetting(setting);
        }

        //private void UpdateRateOfOddsValue(IServiceProvider serviceProvider, long matchId, int betKindId, Dictionary<int, decimal> dictRate)
        //{
        //    Task.Run(async () =>
        //    {
        //        var processOddsService = serviceProvider.GetService<IProcessOddsService>();
        //        await processOddsService.UpdateRateOfOddsValue(matchId, betKindId, dictRate);
        //    });
        //}
    }
}
