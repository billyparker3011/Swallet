namespace SWallet.Core.Models.Discounts
{
    public class DiscountDepositSettingModel
    {
        public int NoOfApplyDiscount { get; set; }  //  So lan ap dung discount
        public int AmountType { get; set; } //  So tien co dinh hoac theo % - Enum AmountType
        public decimal Amount { get; set; } //  So tien duoc huong hoac so phan tram duoc huong
    }
}
