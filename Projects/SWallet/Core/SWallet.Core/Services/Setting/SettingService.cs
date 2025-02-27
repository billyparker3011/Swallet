﻿using HnMicro.Framework.Exceptions;
using HnMicro.Framework.Services;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using SWallet.Core.Contexts;
using SWallet.Core.Converters;
using SWallet.Core.Models.Settings;
using SWallet.Data.Repositories.Settings;
using SWallet.Data.UnitOfWorks;

namespace SWallet.Core.Services.Setting
{
    public class SettingService : SWalletBaseService<SettingService>, ISettingService
    {
        public SettingService(ILogger<SettingService> logger, IServiceProvider serviceProvider, IConfiguration configuration, IClockService clockService, ISWalletClientContext clientContext, ISWalletUow sWalletUow) : base(logger, serviceProvider, configuration, clockService, clientContext, sWalletUow)
        {
        }

        public async Task<SettingModel> GetSetting()
        {
            var settingRepository = SWalletUow.GetRepository<ISettingRepository>();
            var setting = await settingRepository.GetActualSetting();
            return setting == null
                ? throw new NotFoundException()
                : setting.ToSettingModel();
        }

        public async Task UpdateSetting(SettingModel model)
        {
            var settingRepository = SWalletUow.GetRepository<ISettingRepository>();
            var setting = await settingRepository.GetActualSetting() ?? throw new NotFoundException();

            setting.MaskCharacter = model.Mask.MaskCharacter;
            setting.NumberOfMaskCharacters = model.Mask.NumberOfMaskCharacters;

            setting.CurrencySymbol = model.Currency.CurrencySymbol;

            setting.PaymentPartner = model.PaymentPartner;

            setting.MainDomain = model.MainDomain;

            setting.UpdatedAt = ClockService.GetUtcNow();
            setting.UpdatedBy = ClientContext.Manager.ManagerId;

            await SWalletUow.SaveChangesAsync();
        }
    }
}
