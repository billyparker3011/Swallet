using HnMicro.Core.Helpers;
using HnMicro.Framework.Enums;
using HnMicro.Framework.Exceptions;
using HnMicro.Framework.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using SWallet.Core.Consts;
using SWallet.Core.Contexts;
using SWallet.Core.Enums;
using SWallet.Core.Helpers;
using SWallet.Core.Models;
using SWallet.Data.Repositories.Managers;
using SWallet.Data.Repositories.Roles;
using SWallet.Data.UnitOfWorks;
using System.Linq.Expressions;

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

        public async Task CreateManager(CreateManagerModel model)
        {
            var managerRepos = SWalletUow.GetRepository<IManagerRepository>();
            var roleRepos = SWalletUow.GetRepository<IRoleRepository>();

            var clientManager = await managerRepos.FindByIdAsync(ClientContext.Manager.ManagerId) ?? throw new NotFoundException();
            var passwordDecoded = model.Password.DecodePassword();

            await ValidateManagerUsernamePassword(managerRepos, model.Username, passwordDecoded);

            var newManager = new Data.Core.Entities.Manager
            {
                Username = model.Username,
                Password = passwordDecoded.Md5(),
                FullName = model.FullName,
                ManagerRole = clientManager.ManagerRole + 1,
                ManagerCode = model.Username.ConvertStringToAscii(),
                State = ManagerState.Open.ToInt(),
                RoleId = await GetManagerRoleId(roleRepos, clientManager.ManagerRole),
                SupermasterId = GetSuperMasterId(clientManager),
                MasterId = GetMasterId(clientManager),
                ParentId = 0,
                CreatedAt = ClockService.GetUtcNow(),
                CreatedBy = clientManager.ManagerId
            };

            await managerRepos.AddAsync(newManager);
            await SWalletUow.SaveChangesAsync();
        }

        private async Task<int> GetManagerRoleId(IRoleRepository roleRepos, int managerRole)
        {
            return managerRole switch
            {
                (int)ManagerRole.Root or (int)ManagerRole.Supermaster or (int)ManagerRole.Master => await roleRepos.FindQueryBy(x => x.RoleCode == RoleConsts.RoleAsAgent).Select(x => x.RoleId).FirstOrDefaultAsync(),
                (int)ManagerRole.Agent => await roleRepos.FindQueryBy(x => x.RoleCode == RoleConsts.RoleAsCustomer).Select(x => x.RoleId).FirstOrDefaultAsync(),
                _ => 0,
            };
        }

        private long GetMasterId(Data.Core.Entities.Manager clientManager)
        {
            if (clientManager.ManagerRole == (int)ManagerRole.Root)
            {
                return 0;
            }
            return clientManager.ManagerRole is (int)ManagerRole.Master ? clientManager.ManagerId : clientManager.MasterId;
        }

        private long GetSuperMasterId(Data.Core.Entities.Manager clientManager)
        {
            if (clientManager.ManagerRole == (int)ManagerRole.Root)
            {
                return 0;
            }
            return clientManager.ManagerRole is (int)ManagerRole.Supermaster ? clientManager.ManagerId : clientManager.SupermasterId;
        }

        private async Task ValidateManagerUsernamePassword(IManagerRepository managerRepos, string username, string passwordDecoded)
        {
            var existedUsername = await managerRepos.FindByUsernameAndPassword(username, passwordDecoded.Md5());
            var isEnoughComlex = passwordDecoded.IsStrongPassword();
            if (existedUsername != null)
                throw new BadRequestException(ErrorCodeHelper.Manager.UsernameIsExists);
            if (!isEnoughComlex)
                throw new BadRequestException(ErrorCodeHelper.ChangeInfo.PasswordComplexityIsWeak);
        }

        public async Task<GetManagersResult> GetManagers(GetManagersModel model)
        {
            var managerRepos = SWalletUow.GetRepository<IManagerRepository>();

            var targetManagerId = model.ManagerId.HasValue ? model.ManagerId.Value : ClientContext.Manager.ManagerId;

            var clientManager = await managerRepos.FindByIdAsync(targetManagerId) ?? throw new NotFoundException();

            return await GetManagerByRole(managerRepos, clientManager, model);
        }

        private async Task<GetManagersResult> GetManagerByRole(IManagerRepository managerRepos, Data.Core.Entities.Manager clientManager, GetManagersModel model)
        {
            IQueryable<Data.Core.Entities.Manager> managerQuery = managerRepos.FindQuery().Include(f => f.ManagerSession).Include(x => x.Role);

            switch (clientManager.ManagerRole.ToEnum<ManagerRole>())
            {
                case ManagerRole.Root:
                    managerQuery = managerQuery.Where(x => x.ManagerRole == ManagerRole.Supermaster.ToInt() && x.ParentId == 0L);
                    break;
                case ManagerRole.Supermaster:
                    managerQuery = managerQuery.Where(x => x.ManagerRole == ManagerRole.Master.ToInt() && x.SupermasterId == clientManager.ManagerId && x.ParentId == 0L);
                    break;
                case ManagerRole.Master:
                    managerQuery = managerQuery.Where(x => x.ManagerRole == ManagerRole.Agent.ToInt() && x.SupermasterId == clientManager.SupermasterId && x.MasterId == clientManager.ManagerId && x.ParentId == 0L);
                    break;
                default:
                    break;
            }

            if (!string.IsNullOrEmpty(model.SearchTerm))
            {
                managerQuery = managerQuery.Where(x =>
                        x.Username.Contains(model.SearchTerm) ||
                        x.FullName.Contains(model.SearchTerm));
            }
            if (model.State.HasValue)
            {
                managerQuery = managerQuery.Where(x => x.State == model.State.Value);
            }

            if (model.SortType == SortType.Descending)
            {
                managerQuery = model.SortName == "state" ? managerQuery.OrderByDescending(x => x.State).ThenBy(x => x.Username) : managerQuery.OrderByDescending(GetSortManagerProperty(model));
            }
            else
            {
                managerQuery = model.SortName == "state" ? managerQuery.OrderBy(x => x.State).ThenBy(x => x.Username) : managerQuery.OrderBy(GetSortManagerProperty(model));
            }
            var result = await managerRepos.PagingByAsync(managerQuery, model.PageIndex, model.PageSize);
            return new GetManagersResult
            {
                Managers = result.Items.Select(x => new ManagerModel
                {
                    ManagerId = x.ManagerId,
                    ManagerRole = x.ManagerRole,
                    ManagerCode = x.ManagerCode,
                    FullName = x.FullName,
                    RoleCode = x.Role?.RoleCode,
                    RoleName = x.Role?.RoleName,
                    RoleId = x.RoleId,
                    State = x.State.ToEnum<ManagerState>(),
                    Username = x.Username,
                    CreatedDate = x.CreatedAt,
                    IpAddress = x.ManagerSession?.IpAddress,
                    Platform = x.ManagerSession?.Platform,
                    SupermasterId = x.SupermasterId,
                    MasterId = x.MasterId
                }).ToList(),
                Metadata = new HnMicro.Framework.Responses.ApiResponseMetadata
                {
                    NoOfPages = result.Metadata.NoOfPages,
                    NoOfRows = result.Metadata.NoOfRows,
                    NoOfRowsPerPage = result.Metadata.NoOfRowsPerPage,
                    Page = result.Metadata.Page
                }
            };
        }

        private static Expression<Func<Data.Core.Entities.Manager, object>> GetSortManagerProperty(GetManagersModel model)
        {
            if (string.IsNullOrEmpty(model.SortName)) return manager => manager.State;
            return model.SortName?.ToLower() switch
            {
                "username" => manager => manager.Username,
                "createddate" => manager => manager.CreatedAt,
                "state" => manager => manager.State,
                "fullname" => manager => manager.FullName,
                _ => manager => manager
            };
        }
    }
}
