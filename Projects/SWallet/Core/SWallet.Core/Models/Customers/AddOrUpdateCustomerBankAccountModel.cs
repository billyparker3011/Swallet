﻿namespace SWallet.Core.Models
{
    public class AddOrUpdateCustomerBankAccountModel
    {
        public string NumberAccount { get; set; }
        public string CardHolder { get; set; }
        public int BankId { get; set; }
    }
}
