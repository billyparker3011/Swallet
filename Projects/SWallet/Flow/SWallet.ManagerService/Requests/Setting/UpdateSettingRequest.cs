namespace SWallet.ManagerService.Requests.Setting
{
    public class UpdateSettingRequest
    {
        public MaskSettingRequest Mask { get; set; }
        public CurrencySettingRequest Currency { get; set; }
        public int PaymentPartner { get; set; }
        public string MainDomain { get; set; }
    }
}
