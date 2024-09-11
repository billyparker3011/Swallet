using HnMicro.Framework.Services;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using SWallet.Core.Contexts;
using SWallet.Data.UnitOfWorks;

namespace SWallet.Core.Services
{
    public class SWalletBaseService<T> : HnMicroBaseService<T>
    {
        protected ISWalletClientContext ClientContext;
        protected ISWalletUow SWalletUow;

        public SWalletBaseService(ILogger<T> logger, IServiceProvider serviceProvider, IConfiguration configuration, IClockService clockService, ISWalletClientContext clientContext, ISWalletUow sWalletUow) : base(logger, serviceProvider, configuration, clockService)
        {
            ClientContext = clientContext;
            SWalletUow = sWalletUow;
        }
    }
}
