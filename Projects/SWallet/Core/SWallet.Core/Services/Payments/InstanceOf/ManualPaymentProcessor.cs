using HnMicro.Core.Helpers;
using HnMicro.Framework.Exceptions;
using HnMicro.Framework.Services;
using Microsoft.Extensions.Configuration;
using SWallet.Core.Converters;
using SWallet.Core.Enums;
using SWallet.Core.Models.Payment;
using SWallet.Data.Repositories.Banks;
using SWallet.Data.Repositories.Customers;
using SWallet.Data.Repositories.Payments;
using SWallet.Data.Repositories.Transactions;
using SWallet.Data.UnitOfWorks;

namespace SWallet.Core.Services.Payments.InstanceOf
{
    public class ManualPaymentProcessor : InstanceOfPaymentProcessor
    {
        public ManualPaymentProcessor(IServiceProvider serviceProvider, IConfiguration configuration,
            IClockService clockService, ISWalletUow sWalletUow) : base(serviceProvider, configuration, clockService, sWalletUow)
        {
        }

        public override PaymentPartner PaymentPartner => PaymentPartner.Manual;

        public override string PaymentMethodCode => InstanceOfPayment.Manual.Config.InternetBankingCode;

        public override async Task Deposit(long customerId, DepositActivityModel model)
        {
            var bankRepository = SWalletUow.GetRepository<IBankRepository>();
            var bank = await bankRepository.FindByIdAsync(model.BankId) ?? throw new NotFoundException();

            var bankAccountRepository = SWalletUow.GetRepository<IBankAccountRepository>();
            var bankAccount = await bankAccountRepository.FindByBankAndBankAccount(model.BankId, model.BankAccountId);

            var paymentMethodRepository = SWalletUow.GetRepository<IPaymentMethodRepository>();
            var paymentMethod = await paymentMethodRepository.FindByPaymentPartnerAndPaymentMethodCode(PaymentPartner.ToInt(), model.PaymentMethodCode) ?? throw new NotFoundException();

            var customerBankAccountRepository = SWalletUow.GetRepository<ICustomerBankAccountRepository>();
            var customerBankAccount = await customerBankAccountRepository.FindByIdAndCustomer(model.CustomerBankAccountId, customerId) ?? throw new NotFoundException();

            var transactionRepository = SWalletUow.GetRepository<ITransactionRepository>();
            transactionRepository.Add(new Data.Core.Entities.Transaction
            {
                Amount = model.Amount,
                CreatedAt = ClockService.GetUtcNow(),
                CreatedBy = customerId,
                CustomerId = customerId,
                DepositPaymentPartnerId = PaymentPartner.ToInt(),
                DepositPaymentMethodId = paymentMethod.Id,

                DepositBankName = customerBankAccount.Bank.Name,
                DepositCardHolder = customerBankAccount.CardHolder,
                DepositNumberAccount = customerBankAccount.NumberAccount,

                DepositToBankName = bank.Name,
                DepositToCardHolder = bankAccount.CardHolder,
                DepositToNumberAccount = bankAccount.NumberAccount,

                DepositContent = model.Content,
                OriginAmount = model.Amount,
                TransactionState = TransactionState.Processing.ToInt(),
                TransactionType = TransactionType.Deposit.ToInt()
            });

            await SWalletUow.SaveChangesAsync();
        }

        public override async Task<List<BankAccountForModel>> GetBankAccountsForDeposit(int bankId)
        {
            var bankAccountRepository = SWalletUow.GetRepository<IBankAccountRepository>();
            return (await bankAccountRepository.GetDepositBankAccountByBankId(bankId)).Select(f => f.ToBankAccountForModel()).ToList();
        }

        public override async Task<List<BankForModel>> GetBanksForDeposit()
        {
            var bankRepository = SWalletUow.GetRepository<IBankRepository>();
            return (await bankRepository.GetDepositBanks()).Select(f => f.ToBankForModel()).ToList();
        }

        public override async Task Withdraw(long customerId, WithdrawActivityModel model)
        {
            var paymentMethodRepository = SWalletUow.GetRepository<IPaymentMethodRepository>();
            var paymentMethod = await paymentMethodRepository.FindByPaymentPartnerAndPaymentMethodCode(PaymentPartner.ToInt(), model.PaymentMethodCode) ?? throw new NotFoundException();

            var customerBankAccountRepository = SWalletUow.GetRepository<ICustomerBankAccountRepository>();
            var customerBankAccount = await customerBankAccountRepository.FindByIdAndCustomer(model.CustomerBankAccountId, customerId) ?? throw new NotFoundException();

            var transactionRepository = SWalletUow.GetRepository<ITransactionRepository>();
            transactionRepository.Add(new Data.Core.Entities.Transaction
            {
                Amount = model.Amount,
                CreatedAt = ClockService.GetUtcNow(),
                CreatedBy = customerId,
                CustomerId = customerId,
                DepositPaymentPartnerId = PaymentPartner.ToInt(),
                DepositPaymentMethodId = paymentMethod.Id,

                WithdrawToBankName = customerBankAccount.Bank.Name,
                WithdrawToCardHolder = customerBankAccount.CardHolder,
                WithdrawToNumberAccount = customerBankAccount.NumberAccount,

                OriginAmount = model.Amount,
                TransactionState = TransactionState.Processing.ToInt(),
                TransactionType = TransactionType.Withdraw.ToInt()
            });

            await SWalletUow.SaveChangesAsync();
        }
    }
}
