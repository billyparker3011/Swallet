using HnMicro.Framework.Services;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using SWallet.Core.Contexts;
using SWallet.Data.UnitOfWorks;

namespace SWallet.Core.Services.Manager
{
    public class ManagerService : SWalletBaseService<ManagerService>, IManagerService
    {
        public ManagerService(ILogger<ManagerService> logger, 
            IServiceProvider serviceProvider, 
            IConfiguration configuration, 
            IClockService clockService, 
            ISWalletClientContext clientContext, 
            ISWalletUow sWalletUow) : base(logger, serviceProvider, configuration, clockService, clientContext, sWalletUow)
        {
        }
    }
}
