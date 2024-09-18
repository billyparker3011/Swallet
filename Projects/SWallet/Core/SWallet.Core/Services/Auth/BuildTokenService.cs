using HnMicro.Framework.Models;
using HnMicro.Framework.Services;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using SWallet.Core.Configs;
using SWallet.Core.Contexts;
using SWallet.Core.Models.Customers;
using SWallet.Core.Models.Manager;
using SWallet.Data.UnitOfWorks;
using System.Security.Claims;

namespace SWallet.Core.Services.Auth
{
    public class BuildTokenService : SWalletBaseService<BuildTokenService>, IBuildTokenService
    {
        private readonly IJwtTokenService _jwtTokenService;

        public BuildTokenService(ILogger<BuildTokenService> logger, IServiceProvider serviceProvider, IConfiguration configuration, IClockService clockService, ISWalletClientContext clientContext, ISWalletUow sWalletUow,
            IJwtTokenService jwtTokenService) : base(logger, serviceProvider, configuration, clockService, clientContext, sWalletUow)
        {
            _jwtTokenService = jwtTokenService;
        }

        public JwtToken BuildToken(CustomerModel customerModel, CustomerSessionModel customerSessionModel)
        {
            var claims = new List<Claim>
            {
                new(ClaimConfigs.CustomerClaimConfig.CustomerId, customerModel.CustomerId.ToString()),
                new(ClaimConfigs.Username, customerModel.Username),
                new(ClaimConfigs.RoleId, customerModel.RoleId.ToString()),
                new(ClaimConfigs.CustomerClaimConfig.FirstName, customerModel.FirstName ?? string.Empty),
                new(ClaimConfigs.CustomerClaimConfig.LastName, customerModel.LastName ?? string.Empty),
                new(ClaimConfigs.CustomerClaimConfig.IsAffiliate, customerModel.IsAffiliate.ToString()),
                new(ClaimConfigs.CustomerClaimConfig.Email, customerModel.Email ?? customerModel.Email.ToString()),
                new(ClaimConfigs.CustomerClaimConfig.Telegram, customerModel.Telegram ?? customerModel.Telegram.ToString()),
                new(ClaimConfigs.SupermasterId, customerModel.SupermasterId.ToString()),
                new(ClaimConfigs.MasterId, customerModel.MasterId.ToString()),
                new(ClaimConfigs.AgentId, customerModel.AgentId.ToString()),
                new(ClaimConfigs.Hash, customerSessionModel.Hash ?? string.Empty)
            };

            return _jwtTokenService.BuildToken(claims);
        }

        public JwtToken BuildToken(ManagerModel managerModel, ManagerSessionModel managerSessionModel)
        {
            var claims = new List<Claim>
            {
                new(ClaimConfigs.ManagerClaimConfig.ManagerId, managerModel.ManagerId.ToString()),
                new(ClaimConfigs.ManagerClaimConfig.ParentId, managerModel.ParentId.ToString()),
                new(ClaimConfigs.Username, managerModel.Username),
                new(ClaimConfigs.RoleId, managerModel.RoleId.ToString()),
                new(ClaimConfigs.ManagerClaimConfig.ManagerRole, managerModel.ManagerRole.ToString()),
                new(ClaimConfigs.FullName, managerModel.FullName ?? string.Empty),
                //new(ClaimConfigs.NeedToChangePassword, isExpiredPassword.ToString().ToLower()),
                //new(ClaimConfigs.NeedToChangeSecurityCode, isExpiredSecurityCode.ToString().ToLower()),
                new(ClaimConfigs.SupermasterId, managerModel.SupermasterId.ToString()),
                new(ClaimConfigs.MasterId, managerModel.MasterId.ToString()),
                new(ClaimConfigs.Hash, managerSessionModel.Hash)
            };

            throw new NotImplementedException();
        }
    }
}
