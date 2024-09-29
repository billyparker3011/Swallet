using SWallet.Core.Models.Settings;
using SWallet.Data.Core.Entities;

namespace SWallet.Core.Converters
{
    public static class SettingConverter
    {
        public static SettingModel ToSettingModel(this Setting setting)
        {
            return new SettingModel
            {
                Currency = new CurrencySettingModel
                {
                    CurrencySymbol = setting.CurrencySymbol
                },
                Mask = new MaskSettingModel
                {
                    MaskCharacter = setting.MaskCharacter,
                    NumberOfMaskCharacters = setting.NumberOfMaskCharacters
                },
                DateTimeOffset = setting.DateTimeOffSet,
                PaymentPartner = setting.PaymentPartner
            };
        }
    }
}