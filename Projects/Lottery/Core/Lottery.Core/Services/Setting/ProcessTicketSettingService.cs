using HnMicro.Core.Helpers;
using HnMicro.Framework.Services;
using HnMicro.Modules.InMemory.UnitOfWorks;
using Lottery.Core.Contexts;
using Lottery.Core.Enums;
using Lottery.Core.InMemory.Channel;
using Lottery.Core.InMemory.Region;
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
        private readonly IInMemoryUnitOfWork _inMemoryUnitOfWork;
        private readonly IPublishCommonService _publishCommonService;

        public ProcessTicketSettingService(ILogger<ProcessTicketSettingService> logger, IServiceProvider serviceProvider, IConfiguration configuration, IClockService clockService, ILotteryClientContext clientContext, ILotteryUow lotteryUow,
            IInMemoryUnitOfWork inMemoryUnitOfWork,
            IPublishCommonService publishCommonService) : base(logger, serviceProvider, configuration, clockService, clientContext, lotteryUow)
        {
            _inMemoryUnitOfWork = inMemoryUnitOfWork;
            _publishCommonService = publishCommonService;
        }

        public async Task<ScanWaitingTicketSettingModel> GetScanWaitingTicketSetting()
        {
            var settingRepository = LotteryUow.GetRepository<ISettingRepository>();
            var setting = await settingRepository.FindByKey(nameof(ScanWaitingTicketSettingModel));
            if (setting == null || string.IsNullOrEmpty(setting.ValueSetting)) return ScanWaitingTicketSettingModel.CreateDefault();
            return Newtonsoft.Json.JsonConvert.DeserializeObject<ScanWaitingTicketSettingModel>(setting.ValueSetting);
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

        public async Task<ValidationPrizeSettingModel> GetValidationPrizeSetting(int betKindId)
        {
            var key = $"{nameof(ValidationPrizeSettingModel)}|{betKindId}";
            var settingRepository = LotteryUow.GetRepository<ISettingRepository>();
            var setting = await settingRepository.FindByKey(key);
            if (setting == null || string.IsNullOrEmpty(setting.ValueSetting)) return new ValidationPrizeSettingModel { BetKindId = betKindId };
            return Newtonsoft.Json.JsonConvert.DeserializeObject<ValidationPrizeSettingModel>(setting.ValueSetting);
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

        public async Task<ChannelsForCompletedTicketModel> GetChannelsForCompletedTicket()
        {
            var settingRepository = LotteryUow.GetRepository<ISettingRepository>();
            var setting = await settingRepository.FindByKey(nameof(ChannelsForCompletedTicketModel));
            if (setting == null || string.IsNullOrEmpty(setting.ValueSetting)) return ChannelsForCompletedTicketModel.CreateDefault();
            return Newtonsoft.Json.JsonConvert.DeserializeObject<ChannelsForCompletedTicketModel>(setting.ValueSetting);
        }

        public async Task UpdateChannelsForCompletedTicket(ChannelsForCompletedTicketModel model)
        {
            if (model.Items.Count == 0) return;

            var regionInMemoryRepository = _inMemoryUnitOfWork.GetRepository<IRegionInMemoryRepository>();
            var regionIds = model.Items.Keys.ToList();
            var listRegion = regionInMemoryRepository.FindBy(f => regionIds.Contains(f.Id.ToInt()));
            if (regionIds.Count != listRegion.Count()) return;

            var channelInMemoryRepository = _inMemoryUnitOfWork.GetRepository<IChannelInMemoryRepository>();
            var allChannels = channelInMemoryRepository.GetAll().ToList();
            foreach (var itemRegion in model.Items)
            {
                foreach (var itemChannel in itemRegion.Value)
                {
                    foreach (var channelId in itemChannel.ChannelIds)
                    {
                        var channel = allChannels.FirstOrDefault(f => f.Id == channelId);
                        if (channel != null) continue;
                        return;
                    }
                }
            }

            var settingRepository = LotteryUow.GetRepository<ISettingRepository>();
            var setting = await settingRepository.FindByKey(nameof(ChannelsForCompletedTicketModel));
            if (setting == null)
            {
                setting = new Data.Entities.Setting
                {
                    Category = CategoryOfSetting.ProcessTicket.ToInt(),
                    KeySetting = nameof(ChannelsForCompletedTicketModel),
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
