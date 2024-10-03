using HnMicro.Core.Helpers;
using HnMicro.Framework.Exceptions;
using HnMicro.Framework.Services;
using Lottery.Core.Contexts;
using Lottery.Core.Dtos.Setting;
using Lottery.Core.Enums;
using Lottery.Core.Helpers;
using Lottery.Core.Localizations;
using Lottery.Core.Models.Setting;
using Lottery.Core.Models.Setting.BetKind;
using Lottery.Core.Repositories.Setting;
using Lottery.Core.Services.Pubs;
using Lottery.Core.UnitOfWorks;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;

namespace Lottery.Core.Services.Setting
{
    public class BalanceTableSettingService : LotteryBaseService<BalanceTableSettingService>, IBalanceTableSettingService
    {
        private readonly IPublishCommonService _publishCommonService;

        public BalanceTableSettingService(ILogger<BalanceTableSettingService> logger, IServiceProvider serviceProvider, IConfiguration configuration, IClockService clockService, ILotteryClientContext clientContext, ILotteryUow lotteryUow,
            IPublishCommonService publishCommonService) : base(logger, serviceProvider, configuration, clockService, clientContext, lotteryUow)
        {
            _publishCommonService = publishCommonService;
        }

        public string CreateBalanceTableKey(int betKindId)
        {
            return $"DefaultBalanceBetKindTable_{betKindId}";
        }

        public async Task CreateOrModifyBetKindBalanceTableSetting(int betKindId, BalanceTableModel detailSetting)
        {
            var settingRepos = LotteryUow.GetRepository<ISettingRepository>();
            var defaultList = CreateDefault(betKindId.GetNoOfNumbers());
            foreach (var item in detailSetting.ByNumbers.RateValues)
            {
                if (!item.Applied) continue;
                if (detailSetting.ByNumbers.Numbers.Count == 0) continue;
                if (betKindId.IsMixed()) item.AppliedNumbers.Add(-1);
                else item.AppliedNumbers.AddRange(detailSetting.ByNumbers.Numbers);
            }
            foreach (var item in detailSetting.ForCommon.RateValues)
            {
                if (!item.Applied) continue;
                if (betKindId.IsMixed()) item.AppliedNumbers.Add(-1);
                else item.AppliedNumbers.AddRange(defaultList);
            }
            var balanceTableSettingKey = CreateBalanceTableKey(betKindId);
            var existingBetKindBalanceTable = await settingRepos.FindQueryBy(x => x.KeySetting == balanceTableSettingKey && x.Category == CategoryOfSetting.BalanceTable.ToInt()).FirstOrDefaultAsync();
            if (existingBetKindBalanceTable == null)
            {
                await settingRepos.AddAsync(new Data.Entities.Setting
                {
                    KeySetting = balanceTableSettingKey,
                    Category = CategoryOfSetting.BalanceTable.ToInt(),
                    ValueSetting = JsonConvert.SerializeObject(detailSetting),
                    CreatedBy = ClientContext.Agent.AgentId,
                    CreatedAt = ClockService.GetUtcNow()
                });
            }
            else
            {
                existingBetKindBalanceTable.ValueSetting = JsonConvert.SerializeObject(detailSetting);
                existingBetKindBalanceTable.UpdatedBy = ClientContext.Agent.AgentId;
                existingBetKindBalanceTable.UpdatedAt = ClockService.GetUtcNow();
            }

            await LotteryUow.SaveChangesAsync();

            await _publishCommonService.PublishSetting(new SettingModel
            {
                Id = existingBetKindBalanceTable.Id,
                Category = existingBetKindBalanceTable.Category.ToEnum<CategoryOfSetting>(),
                KeySetting = existingBetKindBalanceTable.KeySetting,
                ValueSetting = existingBetKindBalanceTable.ValueSetting
            });
        }

        public async Task<BalanceTableDto> GetBetKindBalanceTableSetting(int betKindId)
        {
            var settingRepos = LotteryUow.GetRepository<ISettingRepository>();

            var balanceTableSettingKey = CreateBalanceTableKey(betKindId);
            var existingBetKindBalanceTable = await settingRepos.FindQueryBy(x => x.KeySetting == balanceTableSettingKey && x.Category == CategoryOfSetting.BalanceTable.ToInt()).FirstOrDefaultAsync();
            if (existingBetKindBalanceTable is null) return new BalanceTableDto();

            var balanceTableSetting = !string.IsNullOrEmpty(existingBetKindBalanceTable.ValueSetting) && existingBetKindBalanceTable.ValueSetting.IsValidJson()
                ? JsonConvert.DeserializeObject<BalanceTableModel>(existingBetKindBalanceTable.ValueSetting)
                : new BalanceTableModel();
            if (balanceTableSetting == null) throw new BadRequestException(Messages.BalanceTableSetting.ErrorValueBalanceTableSetting);

            return new BalanceTableDto
            {
                ForCommon = balanceTableSetting.ForCommon,
                ByNumbers = new BalanceTableNumberDetailDto
                {
                    Numbers = string.Join(",", balanceTableSetting.ByNumbers?.Numbers),
                    RateValues = balanceTableSetting.ByNumbers?.RateValues
                }
            };
        }

        private List<int> CreateDefault(int noOfNumbers)
        {
            var d = new List<int>();
            for (var i = 0; i < noOfNumbers; i++) d.Add(i);
            return d;
        }

        public decimal GetRealValue(decimal unitValue)
        {
            return 1000 * unitValue;
        }

        public void UpdateSetting(SettingModel setting)
        {
            if (setting.Id == 0) return;

            var settingRepository = LotteryUow.GetRepository<ISettingRepository>();
            var existingSetting = settingRepository.FindById(setting.Id);
            if (existingSetting == null) return;

            existingSetting.ValueSetting = setting.ValueSetting;
            existingSetting.UpdatedAt = ClockService.GetUtcNow();
            LotteryUow.SaveChanges();

            Task.Run(async () =>
            {
                await _publishCommonService.PublishSetting(new SettingModel
                {
                    Id = existingSetting.Id,
                    Category = existingSetting.Category.ToEnum<CategoryOfSetting>(),
                    KeySetting = existingSetting.KeySetting,
                    ValueSetting = existingSetting.ValueSetting
                });
            });
        }
    }
}
