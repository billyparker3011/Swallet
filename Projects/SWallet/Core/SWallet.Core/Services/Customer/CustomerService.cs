using HnMicro.Framework.Exceptions;
using HnMicro.Framework.Services;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using SWallet.Core.Contexts;
using SWallet.Core.Converters;
using SWallet.Core.Models.Customers;
using SWallet.Data.Repositories.Customers;
using SWallet.Data.Repositories.Settings;
using SWallet.Data.UnitOfWorks;

namespace SWallet.Core.Services.Customer
{
    public class CustomerService : SWalletBaseService<CustomerService>, ICustomerService
    {
        public CustomerService(ILogger<CustomerService> logger,
            IServiceProvider serviceProvider,
            IConfiguration configuration,
            IClockService clockService,
            ISWalletClientContext clientContext,
            ISWalletUow sWalletUow) : base(logger, serviceProvider, configuration, clockService, clientContext, sWalletUow)
        {
        }

        public async Task ChangeInfo(ChangeInfoModel model)
        {
            var customerRepository = SWalletUow.GetRepository<ICustomerRepository>();
            var customerId = model.CustomerId == 0L ? ClientContext.Customer.CustomerId : model.CustomerId;

            var customer = await customerRepository.FindByIdAsync(customerId) ?? throw new NotFoundException();

            customer.FirstName = model.FirstName ?? customer.FirstName;
            customer.LastName = model.LastName ?? customer.LastName;
            customer.Phone = model.Phone ?? customer.Phone;
            customer.Telegram = model.Telegram ?? customer.Telegram;
            customer.Lock = model.IsLock ?? customer.Lock;
            customer.State = model.State ?? customer.State;

            await SWalletUow.SaveChangesAsync();
        }

        public async Task<MyCustomerProfileModel> CustomerProfile(long customerId = 0L)
        {
            var targetCustomerId = customerId == 0L ? ClientContext.Customer.CustomerId : customerId;
            var customerRepository = SWalletUow.GetRepository<ICustomerRepository>();
            var customer = await customerRepository.FindByIdAsync(targetCustomerId) ?? throw new NotFoundException();
            return customer.ToMyCustomerProfileModel();
        }

        public async Task<MyBalanceCustomerModel> MyBalance()
        {
            var customerId = ClientContext.Customer.CustomerId;
            var balanceCustomerRepository = SWalletUow.GetRepository<IBalanceCustomerRepository>();
            var balance = await balanceCustomerRepository.FindByCustomerId(customerId) ?? throw new NotFoundException();

            var settingRepository = SWalletUow.GetRepository<ISettingRepository>();
            var setting = await settingRepository.GetActualSetting() ?? throw new NotFoundException();

            return balance.ToMyCustomerBalanceModel(setting);
        }
    }
}
