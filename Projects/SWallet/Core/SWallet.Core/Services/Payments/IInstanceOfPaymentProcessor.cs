using SWallet.Core.Enums;
using SWallet.Core.Models.Payment;

namespace SWallet.Core.Services.Payments
{
    public interface IInstanceOfPaymentProcessor
    {
        PaymentPartner PaymentPartner { get; }
        string PaymentMethodCode { get; }

        Task Deposit(long customerId, DepositActivityModel model);
        Task<List<BankAccountForModel>> GetBankAccountsForDeposit(int bankId);
        Task<List<BankForModel>> GetBanksForDeposit();
        Task Withdraw(long customerId, WithdrawActivityModel model);
    }
}
