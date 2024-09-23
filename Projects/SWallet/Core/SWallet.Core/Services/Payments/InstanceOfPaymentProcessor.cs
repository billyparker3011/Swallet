using HnMicro.Framework.Services;
using Microsoft.Extensions.Configuration;
using SWallet.Core.Enums;
using SWallet.Core.Models.Payment;
using SWallet.Data.UnitOfWorks;

namespace SWallet.Core.Services.Payments
{
    public abstract class InstanceOfPaymentProcessor : IInstanceOfPaymentProcessor
    {
        protected IServiceProvider ServiceProvider;
        protected IConfiguration Configuration;
        protected IClockService ClockService;
        protected ISWalletUow SWalletUow;

        protected InstanceOfPaymentProcessor(IServiceProvider serviceProvider,
            IConfiguration configuration,
            IClockService clockService,
            ISWalletUow sWalletUow)
        {
            ServiceProvider = serviceProvider;
            Configuration = configuration;
            ClockService = clockService;
            SWalletUow = sWalletUow;
        }

        public abstract PaymentPartner PaymentPartner { get; }

        public abstract string PaymentMethodCode { get; }

        public abstract Task Deposit(long customerId, DepositActivityModel model);

        public abstract Task<List<BankAccountForModel>> GetBankAccountsForDeposit(int bankId);

        public abstract Task<List<BankForModel>> GetBanksForDeposit();

        public abstract Task Withdraw(long customerId, WithdrawActivityModel model);
    }
}
