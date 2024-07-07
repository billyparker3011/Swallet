using HnMicro.Core.Helpers;
using HnMicro.Framework.Exceptions;
using HnMicro.Framework.Services;
using Lottery.Core.Contexts;
using Lottery.Core.Dtos.Setting;
using Lottery.Core.Enums;
using Lottery.Core.Localizations;
using Lottery.Core.Models.Setting;
using Lottery.Core.Models.Setting.BetKind;
using Lottery.Core.Repositories.Agent;
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
            var agentRepos = LotteryUow.GetRepository<IAgentRepository>();
            var settingRepos = LotteryUow.GetRepository<ISettingRepository>();
            var clientAgent = await agentRepos.FindByIdAsync(ClientContext.Agent.AgentId) ?? throw new NotFoundException();

            var balanceTableSettingKey = CreateBalanceTableKey(betKindId);
            var existingBetKindBalanceTable = await settingRepos.FindQueryBy(x => x.KeySetting == balanceTableSettingKey && x.Category == CategoryOfSetting.BalanceTable.ToInt()).FirstOrDefaultAsync();
            if(existingBetKindBalanceTable == null)
            {
                await settingRepos.AddAsync(new Data.Entities.Setting
                {
                    KeySetting = balanceTableSettingKey,
                    Category = CategoryOfSetting.BalanceTable.ToInt(),
                    ValueSetting = JsonConvert.SerializeObject(detailSetting),
                    CreatedBy = clientAgent.AgentId,
                    CreatedAt = ClockService.GetUtcNow()
                });
            }
            else
            {
                existingBetKindBalanceTable.ValueSetting = JsonConvert.SerializeObject(detailSetting);
                existingBetKindBalanceTable.UpdatedBy = clientAgent.AgentId;
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
            var agentRepos = LotteryUow.GetRepository<IAgentRepository>();
            var settingRepos = LotteryUow.GetRepository<ISettingRepository>();
            var clientAgent = await agentRepos.FindByIdAsync(ClientContext.Agent.AgentId) ?? throw new NotFoundException();

            var balanceTableSettingKey = CreateBalanceTableKey(betKindId);
            var existingBetKindBalanceTable = await settingRepos.FindQueryBy(x => x.KeySetting == balanceTableSettingKey && x.Category == CategoryOfSetting.BalanceTable.ToInt()).FirstOrDefaultAsync();
            if (existingBetKindBalanceTable is null) return new BalanceTableDto();

            var balanceTableSetting = !string.IsNullOrEmpty(existingBetKindBalanceTable.ValueSetting) && existingBetKindBalanceTable.ValueSetting.IsValidJson() 
                ? JsonConvert.DeserializeObject<BalanceTableModel>(existingBetKindBalanceTable.ValueSetting) 
                : new BalanceTableModel();

            if(balanceTableSetting == null) throw new BadRequestException(Messages.BalanceTableSetting.ErrorValueBalanceTableSetting);

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
    }
}
