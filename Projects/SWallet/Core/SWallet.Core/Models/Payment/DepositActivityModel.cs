﻿namespace SWallet.Core.Models.Payment
{
    public class DepositActivityModel
    {
        public long CustomerBankAccountId { get; set; }
        public string PaymentMethodCode { get; set; }
        public int BankId { get; set; }
        public int BankAccountId { get; set; }
        public decimal Amount { get; set; }
        public string Content { get; set; }
    }
}
