using HnMicro.Framework.Services;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using SWallet.Core.Contexts;
using SWallet.Core.Services.Base;
using SWallet.Data.UnitOfWorks;

namespace SWallet.Core.Services.Auth
{
    public class CustomerAuthenticationService : SWalletBaseService<CustomerAuthenticationService>, ICustomerAuthenticationService
    {
        public CustomerAuthenticationService(ILogger<CustomerAuthenticationService> logger, IServiceProvider serviceProvider, IConfiguration configuration, IClockService clockService, ISWalletClientContext sWalletClientContext, ISWalletUow sWalletUow) : base(logger, serviceProvider, configuration, clockService, sWalletClientContext, sWalletUow)
        {
        }
    }
}
