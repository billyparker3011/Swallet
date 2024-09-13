using HnMicro.Core.Helpers;
using HnMicro.Framework.Exceptions;
using HnMicro.Framework.Models;
using HnMicro.Framework.Services;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using SWallet.Core.Configs;
using SWallet.Core.Contexts;
using SWallet.Core.Enums;
using SWallet.Core.Helpers;
using SWallet.Core.Models.Auth;
using SWallet.Data.Repositories.Managers;
using SWallet.Data.UnitOfWorks;
using System.Security.Claims;

namespace SWallet.Core.Services.Auth
{
    public class ManagerAuthenticationService : SWalletBaseService<ManagerAuthenticationService>, IManagerAuthenticationService
    {
        private readonly IJwtTokenService _jwtTokenService;

        public ManagerAuthenticationService(ILogger<ManagerAuthenticationService> logger, IServiceProvider serviceProvider, IConfiguration configuration, IClockService clockService, ISWalletClientContext sWalletClientContext, ISWalletUow sWalletUow,
            IJwtTokenService jwtTokenService) : base(logger, serviceProvider, configuration, clockService, sWalletClientContext, sWalletUow)
        {
            _jwtTokenService = jwtTokenService;
        }

        public async Task<JwtToken> Auth(AuthModel model)
        {
            var managerRepository = SWalletUow.GetRepository<IManagerRepository>();
            var manager = await managerRepository.FindByUsername(model.Username.ToUpper()) ?? throw new BadRequestException(ErrorCodeHelper.Auth.UserPasswordIsWrong);
            if (!manager.Password.Equals(model.Password.DecodePassword().Md5())) throw new BadRequestException(ErrorCodeHelper.Auth.UserPasswordIsWrong);
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

            var claims = new List<Claim>
            {
                new(ClaimConfigs.ManagerClaimConfig.ManagerId, manager.ManagerId.ToString()),
                new(ClaimConfigs.ManagerClaimConfig.ParentId, manager.ParentId.ToString()),
                new(ClaimConfigs.Username, manager.Username),
                new(ClaimConfigs.RoleId, manager.RoleId.ToString()),
                new(ClaimConfigs.ManagerClaimConfig.ManagerRole, manager.ManagerRole.ToString()),
                new(ClaimConfigs.FullName, manager.FullName ?? string.Empty),
                //new(ClaimConfigs.NeedToChangePassword, isExpiredPassword.ToString().ToLower()),
                //new(ClaimConfigs.NeedToChangeSecurityCode, isExpiredSecurityCode.ToString().ToLower()),
                new(ClaimConfigs.SupermasterId, manager.SupermasterId.ToString()),
                new(ClaimConfigs.MasterId, manager.MasterId.ToString()),
                new(ClaimConfigs.Hash, hash)
            };

            return _jwtTokenService.BuildToken(claims);
        }
    }
}
