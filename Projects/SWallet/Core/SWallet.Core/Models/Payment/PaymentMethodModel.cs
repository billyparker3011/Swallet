namespace SWallet.Core.Models.Payment
{
    public class PaymentMethodModel
    {
        public int Id { get; set; }
        public string Code { get; set; }
        public string Name { get; set; }
        public string Icon { get; set; }
        public decimal Fee { get; set; }
        public decimal? Min { get; set; }
        public decimal? Max { get; set; }
        public bool Enabled { get; set; }
    }
}
