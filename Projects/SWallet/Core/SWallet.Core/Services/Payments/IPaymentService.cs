﻿using HnMicro.Core.Scopes;
using SWallet.Core.Models.Enums;
using SWallet.Core.Models.Payment;

namespace SWallet.Core.Services.Payments
{
    public interface IPaymentService : IScopedDependency
    {
        Task<PaymentPartnerInfoModel> GetActualPaymentPartner();
        List<PaymentPartnerInfoModel> GetPaymentPartners();
        Task<List<PaymentMethodModel>> GetPaymentMethods();
        Task<List<PaymentMethodModel>> GetPaymentMethodsBy(int paymentPartnerId);
        Task<List<BankForModel>> GetBanksForDeposit(string paymentMethodCode);
        Task<List<BankAccountForModel>> GetBankAccountsForDeposit(string paymentMethodCode, int bankId);
        Task<List<BankAccountForModel>> GetBankAccountsForWithdraw(int paymentMethodId, int bankId);
        Task<string> GetPaymentContent(string paymentMethodCode);
        Task Deposit(DepositActivityModel model);
        Task Withdraw(WithdrawActivityModel model);
        Task CreatePaymentMethod(CreatePaymentMethodModel model);
    }
}
