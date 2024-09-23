using SWallet.Core.Enums;

namespace SWallet.Core.Models.Enums
{
    public class PaymentPartnerInfoModel
    {
        public PaymentPartner Value { get; set; }
        public string Code { get; set; }
        public string Name { get; set; }
    }
}
