using HnMicro.Framework.Services;
using SWallet.Core.Consts;
using SWallet.Core.Contexts;
using SWallet.Core.Services;
using SWallet.Data.Repositories.Managers;
using SWallet.Data.Repositories.Roles;
using SWallet.Data.UnitOfWorks;
using SWallet.ManagerService.Models.Prepare;

namespace SWallet.ManagerService.Services.Prepare
{
    public class PrepareService : SWalletBaseService<PrepareService>, IPrepareService
    {
        public PrepareService(ILogger<PrepareService> logger, IServiceProvider serviceProvider, IConfiguration configuration, IClockService clockService, ISWalletClientContext clientContext, ISWalletUow sWalletUow) : base(logger, serviceProvider, configuration, clockService, clientContext, sWalletUow)
        {
        }

        public async Task<CreateRootManagerResponseModel> CreateRootManager(CreateRootManagerModel createRootManagerModel)
        {
            ValidationPrepareToken();

            var managerRepository = SWalletUow.GetRepository<IManagerRepository>();
            var managerSessionRepository = SWalletUow.GetRepository<IManagerSessionRepository>();

            managerRepository.Add(new Data.Core.Entities.Manager
            {
                ManagerRole = 0,
                Username = "",
                Password = "",
                FullName = "",
                State = 0,
                RoleId = 0
            });

            throw new NotImplementedException();
        }

        public async Task<bool> InitialRoles()
        {
            ValidationPrepareToken();

            var roleRepository = SWalletUow.GetRepository<IRoleRepository>();
            var listRoles = new List<string> { RoleConsts.RoleAsRoot, RoleConsts.RoleAsManager, RoleConsts.RoleAsAgent, RoleConsts.RoleAsCustomer };
            var roles = await roleRepository.GetRoleByRoleCode(listRoles);
            if (roles.Count > 0) return false;
            foreach (var role in listRoles)
            {
                roleRepository.Add(new Data.Core.Entities.Role
                {
                    RoleCode = role,
                    RoleName = role,
                    CreatedAt = ClockService.GetUtcNow(),
                    CreatedBy = 0L
                });
            }
            return (await SWalletUow.SaveChangesAsync()) > 0;
        }
    }
}
