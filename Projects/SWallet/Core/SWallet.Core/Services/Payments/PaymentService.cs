using HnMicro.Core.Helpers;
using HnMicro.Framework.Exceptions;
using HnMicro.Framework.Services;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using SWallet.Core.Consts;
using SWallet.Core.Contexts;
using SWallet.Core.Converters;
using SWallet.Core.Enums;
using SWallet.Core.Models.Enums;
using SWallet.Core.Models.Payment;
using SWallet.Data.Repositories.Payments;
using SWallet.Data.Repositories.Settings;
using SWallet.Data.UnitOfWorks;

namespace SWallet.Core.Services.Payments
{
    public class PaymentService : SWalletBaseService<PaymentService>, IPaymentService
    {
        private readonly IPaymentProcessor _paymentProcessor;

        public PaymentService(ILogger<PaymentService> logger, IServiceProvider serviceProvider, IConfiguration configuration, IClockService clockService, ISWalletClientContext clientContext, ISWalletUow sWalletUow,
            IPaymentProcessor paymentProcessor) : base(logger, serviceProvider, configuration, clockService, clientContext, sWalletUow)
        {
            _paymentProcessor = paymentProcessor;
        }

        private async Task<Data.Core.Entities.Setting> GetInternalActualPaymentPartner()
        {
            var settingRepository = SWalletUow.GetRepository<ISettingRepository>();
            var setting = await settingRepository.GetActualSetting() ?? throw new NotFoundException();
            if (setting.PaymentPartner == 0) throw new BadRequestException(CommonMessageConsts.PaymentPartnerHasNotBeenInitialYet);
            return setting;
        }

        public async Task<PaymentPartnerInfoModel> GetActualPaymentPartner()
        {
            var setting = await GetInternalActualPaymentPartner();

            var allPaymentPartners = Helpers.EnumHelper.GetListPaymentPartnerInfo();
            var actualPaymentPartners = allPaymentPartners.FirstOrDefault(f => f.Value == setting.PaymentPartner.ToEnum<PaymentPartner>());
            return actualPaymentPartners == null
                ? throw new NotFoundException(CommonMessageConsts.PaymentPartnerDoesNotExist)
                : actualPaymentPartners;
        }

        public async Task<List<BankAccountForModel>> GetBankAccountsForDeposit(string paymentMethodCode, int bankId)
        {
            var setting = await GetInternalActualPaymentPartner();
            return await _paymentProcessor.GetBankAccountsForDeposit(setting.PaymentPartner, paymentMethodCode, bankId);
        }

        public async Task<List<BankForModel>> GetBanksForDeposit(string paymentMethodCode)
        {
            var setting = await GetInternalActualPaymentPartner();
            return await _paymentProcessor.GetBanksForDeposit(setting.PaymentPartner, paymentMethodCode);
        }

        public async Task<List<PaymentMethodModel>> GetPaymentMethods()
        {
            var setting = await GetInternalActualPaymentPartner();

            var paymentMethodRepository = SWalletUow.GetRepository<IPaymentMethodRepository>();
            var enabledPaymentMethods = await paymentMethodRepository.FindEnabledPaymentMethods(setting.PaymentPartner);

            return enabledPaymentMethods.Select(f => f.ToPaymentMethodModel()).ToList();
        }

        public List<PaymentPartnerInfoModel> GetPaymentPartners()
        {
            return Helpers.EnumHelper.GetListPaymentPartnerInfo();
        }

        public async Task<string> GetPaymentContent(string paymentMethodCode)
        {
            var setting = await GetInternalActualPaymentPartner();
            var customerUsername = ClientContext.Customer.UserName;
            return await _paymentProcessor.GetPaymentContent(setting.PaymentPartner, paymentMethodCode, customerUsername);
        }

        public async Task Deposit(DepositActivityModel model)
        {
            var setting = await GetInternalActualPaymentPartner();
            var customerId = ClientContext.Customer.CustomerId;
            await _paymentProcessor.Deposit(setting.PaymentPartner, customerId, model);
        }

        public async Task Withdraw(WithdrawActivityModel model)
        {
            var setting = await GetInternalActualPaymentPartner();
            var customerId = ClientContext.Customer.CustomerId;
            await _paymentProcessor.Withdraw(setting.PaymentPartner, customerId, model);
        }
    }
}
