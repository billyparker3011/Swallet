using SWallet.Core.Models.Bank;
using SWallet.Core.Models.Payment;
using SWallet.Data.Core.Entities;

namespace SWallet.Core.Converters
{
    public static class BankConverter
    {
        public static BankModel ToBankModel(this Bank bank)
        {
            return new BankModel
            {
                BankId = bank.BankId,
                BankName = bank.Name,
                DepositEnabled = bank.DepositEnabled,
                Icon = bank.Icon,
                WithdrawEnabled = bank.WithdrawEnabled
            };
        }

        public static BankForModel ToBankForModel(this Bank bank)
        {
            return new BankForModel
            {
                BankId = bank.BankId,
                BankName = bank.Name,
                Icon = bank.Icon
            };
        }

        public static BankAccountForModel ToBankAccountForModel(this BankAccount bankAccount)
        {
            return new BankAccountForModel
            {
                BankAccountId = bankAccount.BankAccountId,
                BankId = bankAccount.BankId,
                CardHolder = bankAccount.CardHolder,
                NumberAccount = bankAccount.NumberAccount
            };
        }
    }
}