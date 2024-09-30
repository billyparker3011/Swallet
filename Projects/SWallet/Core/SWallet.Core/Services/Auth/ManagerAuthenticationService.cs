using HnMicro.Core.Helpers;
using HnMicro.Framework.Exceptions;
using HnMicro.Framework.Models;
using HnMicro.Framework.Services;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using SWallet.Core.Contexts;
using SWallet.Core.Converters;
using SWallet.Core.Enums;
using SWallet.Core.Helpers;
using SWallet.Core.Models.Auth;
using SWallet.Data.Repositories.Managers;
using SWallet.Data.UnitOfWorks;

namespace SWallet.Core.Services.Auth
{
    public class ManagerAuthenticationService : SWalletBaseService<ManagerAuthenticationService>, IManagerAuthenticationService
    {
        private readonly IJwtTokenService _jwtTokenService;
        private readonly IBuildTokenService _buildTokenService;

        public ManagerAuthenticationService(ILogger<ManagerAuthenticationService> logger, IServiceProvider serviceProvider, IConfiguration configuration, IClockService clockService, ISWalletClientContext sWalletClientContext, ISWalletUow sWalletUow,
            IBuildTokenService buildTokenService) : base(logger, serviceProvider, configuration, clockService, sWalletClientContext, sWalletUow)
        {
            _buildTokenService = buildTokenService;
        }

        public async Task<JwtToken> Auth(AuthModel model)
        {
            var managerRepository = SWalletUow.GetRepository<IManagerRepository>();
            var manager = await managerRepository.FindByUsername(model.Username.ToUpper()) ?? throw new BadRequestException(ErrorCodeHelper.Auth.UsernameOrPasswordIsWrong);
            if (!manager.Password.Equals(model.Password.DecodePassword().Md5())) throw new BadRequestException(ErrorCodeHelper.Auth.UsernameOrPasswordIsWrong);
            if (manager.State == ManagerState.Closed.ToInt()) throw new BadRequestException(ErrorCodeHelper.Auth.UserClosed);

            var clientInformation = ClientContext.GetClientInformation();

            var hash = StringHelper.MaxHashLength.RandomString();
            var managerSessionRepository = SWalletUow.GetRepository<IManagerSessionRepository>();
            var managerSession = await managerSessionRepository.FindByManagerId(manager.ManagerId);
            if (managerSession == null)
            {
                managerSession = new Data.Core.Entities.ManagerSession
                {
                    ManagerId = manager.ManagerId,
                    Hash = hash,
                    IpAddress = clientInformation?.IpAddress,
                    UserAgent = clientInformation?.UserAgent,
                    Platform = clientInformation?.Platform,
                    State = SessionState.Online.ToInt(),
                    LatestDoingTime = ClockService.GetUtcNow()
                };
                managerSessionRepository.Add(managerSession);
            }
            else
            {
                managerSession.Hash = hash;
                managerSession.IpAddress = clientInformation?.IpAddress;
                managerSession.UserAgent = clientInformation?.UserAgent;
                managerSession.Platform = clientInformation?.Platform;
                managerSession.State = SessionState.Online.ToInt();
                managerSession.LatestDoingTime = ClockService.GetUtcNow();
                managerSessionRepository.Update(managerSession);
            }

            await SWalletUow.SaveChangesAsync();

            return _buildTokenService.BuildToken(manager.ToManagerModel(), managerSession.ToManagerSessionModel());
        }
    }
}
