using HnMicro.Framework.Services;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using SWallet.Core.Contexts;
using SWallet.Data.UnitOfWorks;

namespace SWallet.Core.Services.Base
{
    public class SWalletBaseService<T> : HnMicroBaseService<T>
    {
        protected ISWalletClientContext SWalletClientContext;
        protected ISWalletUow SWalletUow;

        public SWalletBaseService(ILogger<T> logger, IServiceProvider serviceProvider, IConfiguration configuration, IClockService clockService, ISWalletClientContext sWalletClientContext, ISWalletUow sWalletUow) : base(logger, serviceProvider, configuration, clockService)
        {
            SWalletClientContext = sWalletClientContext;
            SWalletUow = sWalletUow;
        }
    }
}
