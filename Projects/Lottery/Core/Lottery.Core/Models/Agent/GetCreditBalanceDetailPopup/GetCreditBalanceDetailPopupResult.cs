namespace Lottery.Core.Models.Agent.GetCreditBalanceDetailPopup
{
    public class GetCreditBalanceDetailPopupResult
    {
        public decimal CurrentGivenCredit {  get; set; }
        public decimal GivenCredit { get; set; }
        public decimal MinCredit { get; set; }
        public decimal MaxCredit { get; set; }
        public decimal MinMemberMaxCredit { get; set; }
    }
}
