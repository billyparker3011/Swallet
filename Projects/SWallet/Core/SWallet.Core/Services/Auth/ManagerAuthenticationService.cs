using HnMicro.Framework.Models;
using HnMicro.Framework.Services;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using SWallet.Core.Contexts;
using SWallet.Core.Models.Auth;
using SWallet.Data.UnitOfWorks;

namespace SWallet.Core.Services.Auth
{
    public class ManagerAuthenticationService : SWalletBaseService<ManagerAuthenticationService>, IManagerAuthenticationService
    {
        public ManagerAuthenticationService(ILogger<ManagerAuthenticationService> logger, IServiceProvider serviceProvider, IConfiguration configuration, IClockService clockService, ISWalletClientContext sWalletClientContext, ISWalletUow sWalletUow) : base(logger, serviceProvider, configuration, clockService, sWalletClientContext, sWalletUow)
        {
        }

        public async Task<JwtToken> Auth(AuthModel model)
        {
            return new JwtToken();
        }
    }
}
