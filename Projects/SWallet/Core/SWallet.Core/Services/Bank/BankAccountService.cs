using HnMicro.Framework.Services;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using SWallet.Core.Contexts;
using SWallet.Data.UnitOfWorks;

namespace SWallet.Core.Services.Bank
{
    public class BankAccountService : SWalletBaseService<BankAccountService>, IBankAccountService
    {
        public BankAccountService(ILogger<BankAccountService> logger, 
            IServiceProvider serviceProvider, 
            IConfiguration configuration, 
            IClockService clockService, 
            ISWalletClientContext clientContext, 
            ISWalletUow sWalletUow) : base(logger, serviceProvider, configuration, clockService, clientContext, sWalletUow)
        {
        }
    }
}
