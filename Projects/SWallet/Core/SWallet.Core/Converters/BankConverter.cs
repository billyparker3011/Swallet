using SWallet.Core.Models.Bank;
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
    }
}