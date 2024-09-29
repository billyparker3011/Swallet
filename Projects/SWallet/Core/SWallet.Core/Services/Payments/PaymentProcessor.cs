using HnMicro.Core.Helpers;
using HnMicro.Framework.Exceptions;
using HnMicro.Framework.Services;
using Microsoft.Extensions.Configuration;
using SWallet.Core.Enums;
using SWallet.Core.Models.Payment;
using SWallet.Data.UnitOfWorks;

namespace SWallet.Core.Services.Payments
{
    public class PaymentProcessor : IPaymentProcessor
    {
        private readonly List<IInstanceOfPaymentProcessor> _instances = new();
        private readonly IServiceProvider _serviceProvider;
        private readonly IConfiguration _configuration;
        private readonly IClockService _clockService;
        private readonly ISWalletUow _sWalletUow;

        public PaymentProcessor(IServiceProvider serviceProvider, IConfiguration configuration, IClockService clockService, ISWalletUow sWalletUow)
        {
            _serviceProvider = serviceProvider;
            _configuration = configuration;
            _clockService = clockService;
            _sWalletUow = sWalletUow;

            InitInstanceOfProcessors();
        }

        private void InitInstanceOfProcessors()
        {
            var instanceOfPayments = typeof(IInstanceOfPaymentProcessor).GetDerivedClass();
            foreach (var instance in instanceOfPayments)
            {
                _instances.Add(Activator.CreateInstance(instance, _serviceProvider, _configuration, _clockService, _sWalletUow) as IInstanceOfPaymentProcessor);
            }
        }

        public async Task<List<BankForModel>> GetBanksForDeposit(int paymentPartner, string paymentMethodCode)
        {
            var instanceOf = _instances.FirstOrDefault(f => f.PaymentPartner == paymentPartner.ToEnum<PaymentPartner>() && f.PaymentMethodCode == paymentMethodCode);
            return instanceOf == null ? throw new NotFoundException() : await instanceOf.GetBanksForDeposit();
        }

        public async Task<List<BankAccountForModel>> GetBankAccountsForDeposit(int paymentPartner, string paymentMethodCode, int bankId)
        {
            var instanceOf = _instances.FirstOrDefault(f => f.PaymentPartner == paymentPartner.ToEnum<PaymentPartner>() && f.PaymentMethodCode == paymentMethodCode);
            return instanceOf == null ? throw new NotFoundException() : await instanceOf.GetBankAccountsForDeposit(bankId);
        }

        public virtual async Task<string> GetPaymentContent(int paymentPartner, string paymentMethodCode, string currentUsername)
        {
            var currentTime = _clockService.GetUtcNow();
            var content = currentUsername.RandomStringFrom(3);
            return $"{currentTime.Millisecond}{currentTime.Hour}{currentTime.Year}{currentTime.Minute}{content}{currentTime.Day}{currentTime.Second}{currentTime.Month}";
        }

        public async Task Deposit(int paymentPartner, long customerId, DepositActivityModel model)
        {
            var instanceOf = _instances.FirstOrDefault(f => f.PaymentPartner == paymentPartner.ToEnum<PaymentPartner>() && f.PaymentMethodCode == model.PaymentMethodCode) ?? throw new NotFoundException();
            await instanceOf.Deposit(customerId, model);
        }

        public async Task Withdraw(int paymentPartner, long customerId, WithdrawActivityModel model)
        {
            var instanceOf = _instances.FirstOrDefault(f => f.PaymentPartner == paymentPartner.ToEnum<PaymentPartner>() && f.PaymentMethodCode == model.PaymentMethodCode) ?? throw new NotFoundException();
            await instanceOf.Withdraw(customerId, model);
        }
    }
}
