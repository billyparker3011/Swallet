using HnMicro.Core.Scopes;
using SWallet.Core.Models.Payment;

namespace SWallet.Core.Services.Payments
{
    public interface IPaymentProcessor : IScopedDependency
    {
        Task Deposit(int paymentPartner, long customerId, DepositActivityModel model);
        Task<List<BankAccountForModel>> GetBankAccountsForDeposit(int paymentPartner, string paymentMethodCode, int bankId);
        Task<List<BankForModel>> GetBanksForDeposit(int paymentPartner, string paymentMethodCode);
        Task<string> GetPaymentContent(int paymentPartner, string paymentMethodCode, string currentUsername);
    }
}
