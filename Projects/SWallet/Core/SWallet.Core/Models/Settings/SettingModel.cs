namespace SWallet.Core.Models.Settings
{
    public class SettingModel
    {
        public MaskSettingModel Mask { get; set; }

        public CurrencySettingModel Currency { get; set; }

        public int PaymentPartner { get; set; }

        public int DateTimeOffset { get; set; }
    }
}
