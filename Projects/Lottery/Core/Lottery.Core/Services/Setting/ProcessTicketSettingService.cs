using HnMicro.Core.Helpers;
using HnMicro.Framework.Services;
using Lottery.Core.Contexts;
using Lottery.Core.Enums;
using Lottery.Core.Models.Setting;
using Lottery.Core.Models.Setting.ProcessTicket;
using Lottery.Core.Repositories.Setting;
using Lottery.Core.Services.Pubs;
using Lottery.Core.UnitOfWorks;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace Lottery.Core.Services.Setting
{
    public class ProcessTicketSettingService : LotteryBaseService<ProcessTicketSettingService>, IProcessTicketSettingService
    {
        private readonly IPublishCommonService _publishCommonService;

        public ProcessTicketSettingService(ILogger<ProcessTicketSettingService> logger, IServiceProvider serviceProvider, IConfiguration configuration, IClockService clockService, ILotteryClientContext clientContext, ILotteryUow lotteryUow,
            IPublishCommonService publishCommonService) : base(logger, serviceProvider, configuration, clockService, clientContext, lotteryUow)
        {
            _publishCommonService = publishCommonService;
        }

        public async Task UpdateScanWaitingTicketSetting(ScanWaitingTicketSettingModel model)
        {
            var settingRepository = LotteryUow.GetRepository<ISettingRepository>();
            var setting = await settingRepository.FindByKey(nameof(ScanWaitingTicketSettingModel));
            if (setting == null)
            {
                setting = new Data.Entities.Setting
                {
                    Category = CategoryOfSetting.ProcessTicket.ToInt(),
                    KeySetting = nameof(ScanWaitingTicketSettingModel),
                    ValueSetting = Newtonsoft.Json.JsonConvert.SerializeObject(model),
                    CreatedAt = ClockService.GetUtcNow(),
                    CreatedBy = ClientContext.Agent.AgentId
                };
                settingRepository.Add(setting);
            }
            else
            {
                setting.ValueSetting = Newtonsoft.Json.JsonConvert.SerializeObject(model);
                setting.UpdatedAt = ClockService.GetUtcNow();
                setting.UpdatedBy = ClientContext.Agent.AgentId;
                settingRepository.Update(setting);
            }
            await LotteryUow.SaveChangesAsync();

            await _publishCommonService.PublishSetting(new SettingModel
            {
                Id = setting.Id,
                Category = setting.Category.ToEnum<CategoryOfSetting>(),
                KeySetting = setting.KeySetting,
                ValueSetting = setting.ValueSetting
            });
        }

        public async Task UpdateValidationPrizeSetting(ValidationPrizeSettingModel model)
        {
            var key = $"{nameof(ValidationPrizeSettingModel)}|{model.BetKindId}";
            var settingRepository = LotteryUow.GetRepository<ISettingRepository>();
            var setting = await settingRepository.FindByKey(key);
            if (setting == null)
            {
                setting = new Data.Entities.Setting
                {
                    Category = CategoryOfSetting.ProcessTicket.ToInt(),
                    KeySetting = key,
                    ValueSetting = Newtonsoft.Json.JsonConvert.SerializeObject(model),
                    CreatedAt = ClockService.GetUtcNow(),
                    CreatedBy = ClientContext.Agent.AgentId
                };
                settingRepository.Add(setting);
            }
            else
            {
                setting.ValueSetting = Newtonsoft.Json.JsonConvert.SerializeObject(model);
                setting.UpdatedAt = ClockService.GetUtcNow();
                setting.UpdatedBy = ClientContext.Agent.AgentId;
                settingRepository.Update(setting);
            }
            await LotteryUow.SaveChangesAsync();

            await _publishCommonService.PublishSetting(new SettingModel
            {
                Id = setting.Id,
                Category = setting.Category.ToEnum<CategoryOfSetting>(),
                KeySetting = setting.KeySetting,
                ValueSetting = setting.ValueSetting
            });
        }
    }
}
