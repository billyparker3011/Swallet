using HnMicro.Framework.Exceptions;
using HnMicro.Framework.Services;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using SWallet.Core.Contexts;
using SWallet.Core.Models.Customers;
using SWallet.Data.Repositories.Customers;
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
            var customerId = ClientContext.Customer.CustomerId;

            var customer = await customerRepository.FindByIdAsync(customerId);
            if (customer == null) throw new NotFoundException();

            customer.FirstName = model.FirstName;
            customer.LastName = model.LastName;
            customer.Phone = model.Phone;
            customer.Telegram = model.Telegram;

            await SWalletUow.SaveChangesAsync();
        }
    }
}
