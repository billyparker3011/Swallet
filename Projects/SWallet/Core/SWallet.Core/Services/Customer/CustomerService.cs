using HnMicro.Framework.Services;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using SWallet.Core.Contexts;
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
    }
}
