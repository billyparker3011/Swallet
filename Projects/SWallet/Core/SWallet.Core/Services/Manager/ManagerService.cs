using HnMicro.Core.Helpers;
using HnMicro.Framework.Enums;
using HnMicro.Framework.Exceptions;
using HnMicro.Framework.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Internal;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using SWallet.Core.Consts;
using SWallet.Core.Contexts;
using SWallet.Core.Enums;
using SWallet.Core.Helpers;
using SWallet.Core.Models;
using SWallet.Data.Repositories.Customers;
using SWallet.Data.Repositories.Managers;
using SWallet.Data.Repositories.Roles;
using SWallet.Data.UnitOfWorks;
using System.Linq.Expressions;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory;

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
                (int)ManagerRole.Root => await roleRepos.FindQueryBy(x => x.RoleCode == RoleConsts.RoleAsManager).Select(x => x.RoleId).FirstOrDefaultAsync(),
                (int)ManagerRole.Supermaster => await roleRepos.FindQueryBy(x => x.RoleCode == RoleConsts.RoleAsAgent).Select(x => x.RoleId).FirstOrDefaultAsync(),
                (int)ManagerRole.Master => await roleRepos.FindQueryBy(x => x.RoleCode == RoleConsts.RoleAsAgent).Select(x => x.RoleId).FirstOrDefaultAsync(),
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
            var customerRepos = SWalletUow.GetRepository<ICustomerRepository>();

            var targetManagerId = model.ManagerId.HasValue ? model.ManagerId.Value : ClientContext.Manager.ManagerId;

            var clientManager = await managerRepos.FindByIdAsync(targetManagerId) ?? throw new NotFoundException();

            if (clientManager.ManagerRole.ToEnum<ManagerRole>() == ManagerRole.Agent)
            {
                return await GetCustomersByAgent(customerRepos, clientManager, model);
            }
            return await GetManagerByRole(managerRepos, clientManager, model);
        }

        private async Task<GetManagersResult> GetCustomersByAgent(ICustomerRepository customerRepos, Data.Core.Entities.Manager clientManager, GetManagersModel model)
        {
            IQueryable<Data.Core.Entities.Customer> customerQuery = customerRepos.FindQuery().Include(f => f.CustomerSession);

            customerQuery = customerQuery.Where(x => x.AgentId == clientManager.ManagerId);

            if (!string.IsNullOrEmpty(model.SearchTerm))
            {
                customerQuery = customerQuery.Where(x =>
                        x.Username.Contains(model.SearchTerm) ||
                        x.LastName.Contains(model.SearchTerm) ||
                        x.FirstName.Contains(model.SearchTerm));
            }
            if (model.State.HasValue)
            {
                customerQuery = customerQuery.Where(x => x.State == model.State.Value);
            }

            if (model.SortType == SortType.Descending)
            {
                customerQuery = model.SortName == "state" ? customerQuery.OrderByDescending(x => x.State).ThenBy(x => x.Username) : customerQuery.OrderByDescending(GetSortAgentCustomerProperty(model));
            }
            else
            {
                customerQuery = model.SortName == "state" ? customerQuery.OrderBy(x => x.State).ThenBy(x => x.Username) : customerQuery.OrderBy(GetSortAgentCustomerProperty(model));
            }
            var result = await customerRepos.PagingByAsync(customerQuery, model.PageIndex, model.PageSize);
            return new GetManagersResult
            {
                Managers = result.Items.Select(x => new ManagerModel
                {
                    ManagerId = x.CustomerId,
                    ManagerRole = ManagerRole.Customer.ToInt(),
                    FullName = x.FirstName + " " + x.LastName,
                    RoleCode = x.Role?.RoleCode,
                    RoleName = x.Role?.RoleName,
                    RoleId = x.RoleId,
                    State = x.State,
                    Username = x.Username,
                    CreatedDate = x.CreatedAt,
                    IpAddress = x.CustomerSession?.IpAddress,
                    Platform = x.CustomerSession?.Platform,
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
                    State = x.State,
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

        private static Expression<Func<Data.Core.Entities.Customer, object>> GetSortAgentCustomerProperty(GetManagersModel model)
        {
            if (string.IsNullOrEmpty(model.SortName)) return customer => customer.State;
            return model.SortName?.ToLower() switch
            {
                "username" => customer => customer.Username,
                "createddate" => customer => customer.CreatedAt,
                "state" => customer => customer.State,
                "fullname" => customer => customer.FirstName + " " + customer.LastName,
                _ => customer => customer
            };
        }

        public async Task<GetCustomerOfAgentManagerResult> GetCustomerOfAgentManager(GetCustomerOfAgentManagerModel model)
        {
            var managerRepos = SWalletUow.GetRepository<IManagerRepository>();
            var customerRepos = SWalletUow.GetRepository<ICustomerRepository>();

            var clientManager = await managerRepos.FindByIdAsync(ClientContext.Manager.ManagerId) ?? throw new NotFoundException();

            var customerQuery = clientManager.ManagerRole == ManagerRole.Root.ToInt() ? customerRepos.FindQuery().Include(f => f.CustomerSession).Include(f => f.Role)
                                                                                                                            .GroupJoin(managerRepos.FindQuery(), c => c.AgentId, m => m.ManagerId, (customer, manager) => new
                                                                                                                            {
                                                                                                                                customer,
                                                                                                                                manager
                                                                                                                            })
                                                                                                                            .SelectMany(f => f.manager.DefaultIfEmpty(), (customerRes, managerRes) => new CustomerModel
                                                                                                                            {
                                                                                                                                CustomerId = customerRes.customer.CustomerId,
                                                                                                                                RoleName = customerRes.customer.Role != null ? customerRes.customer.Role.RoleName : null,
                                                                                                                                RoleCode = customerRes.customer.Role != null ? customerRes.customer.Role.RoleCode : null,
                                                                                                                                RoleId = customerRes.customer.RoleId,
                                                                                                                                State = customerRes.customer.State.ToEnum<CustomerState>(),
                                                                                                                                Username = customerRes.customer.Username,
                                                                                                                                UsernameUpper = customerRes.customer.UsernameUpper,
                                                                                                                                FirstName = customerRes.customer.FirstName,
                                                                                                                                LastName = customerRes.customer.LastName,
                                                                                                                                Email = customerRes.customer.Email,
                                                                                                                                Phone = customerRes.customer.Phone,
                                                                                                                                Telegram = customerRes.customer.Telegram,
                                                                                                                                IsAffiliate = customerRes.customer.IsAffiliate,
                                                                                                                                AgentId = customerRes.customer.AgentId,
                                                                                                                                IpAddress = customerRes.customer.CustomerSession != null ? customerRes.customer.CustomerSession.IpAddress : null,
                                                                                                                                Platform = customerRes.customer.CustomerSession != null ? customerRes.customer.CustomerSession.Platform : null,
                                                                                                                                SupermasterId = customerRes.customer.SupermasterId,
                                                                                                                                MasterId = customerRes.customer.MasterId,
                                                                                                                                CreatedDate = customerRes.customer.CreatedAt,
                                                                                                                                AffiliateUsername = managerRes != null ? managerRes.Username : null,
                                                                                                                            })
                                                                                                                        : customerRepos.FindQuery().Include(f => f.CustomerSession).Include(f => f.Role).Where(f => f.IsAffiliate)
                                                                                                                            .GroupJoin(managerRepos.FindQuery(), c => c.AgentId, m => m.ManagerId, (customer, manager) => new
                                                                                                                            {
                                                                                                                                customer,
                                                                                                                                manager
                                                                                                                            })
                                                                                                                            .SelectMany(f => f.manager.DefaultIfEmpty(), (customerRes, managerRes) => new CustomerModel
                                                                                                                            {
                                                                                                                                CustomerId = customerRes.customer.CustomerId,
                                                                                                                                RoleName = customerRes.customer.Role != null ? customerRes.customer.Role.RoleName : null,
                                                                                                                                RoleCode = customerRes.customer.Role != null ? customerRes.customer.Role.RoleCode : null,
                                                                                                                                RoleId = customerRes.customer.RoleId,
                                                                                                                                State = customerRes.customer.State.ToEnum<CustomerState>(),
                                                                                                                                Username = customerRes.customer.Username,
                                                                                                                                UsernameUpper = customerRes.customer.UsernameUpper,
                                                                                                                                FirstName = customerRes.customer.FirstName,
                                                                                                                                LastName = customerRes.customer.LastName,
                                                                                                                                Email = customerRes.customer.Email,
                                                                                                                                Phone = customerRes.customer.Phone,
                                                                                                                                Telegram = customerRes.customer.Telegram,
                                                                                                                                IsAffiliate = customerRes.customer.IsAffiliate,
                                                                                                                                AgentId = customerRes.customer.AgentId,
                                                                                                                                IpAddress = customerRes.customer.CustomerSession != null ? customerRes.customer.CustomerSession.IpAddress : null,
                                                                                                                                Platform = customerRes.customer.CustomerSession != null ? customerRes.customer.CustomerSession.Platform : null,
                                                                                                                                SupermasterId = customerRes.customer.SupermasterId,
                                                                                                                                MasterId = customerRes.customer.MasterId,
                                                                                                                                CreatedDate = customerRes.customer.CreatedAt,
                                                                                                                                AffiliateUsername = managerRes != null ? managerRes.Username : null,
                                                                                                                            });

            switch (clientManager.ManagerRole.ToEnum<ManagerRole>())
            {
                case ManagerRole.Supermaster:
                    customerQuery = customerQuery.Where(x => x.SupermasterId == clientManager.ManagerId);
                    break;
                case ManagerRole.Master:
                    customerQuery = customerQuery.Where(x => x.SupermasterId == clientManager.SupermasterId && x.MasterId == clientManager.ManagerId);
                    break;
                case ManagerRole.Agent:
                    customerQuery = customerQuery.Where(x => x.SupermasterId == clientManager.SupermasterId && x.MasterId == clientManager.MasterId && x.AgentId == clientManager.ManagerId);
                    break;
                default:
                    break;
            }

            if (!string.IsNullOrEmpty(model.SearchTerm))
            {
                customerQuery = customerQuery.Where(x =>
                        x.Username.Contains(model.SearchTerm) ||
                        x.FirstName.Contains(model.SearchTerm) ||
                        x.LastName.Contains(model.SearchTerm));
            }
            if (model.State.HasValue)
            {
                customerQuery = customerQuery.Where(x => x.State == model.State.Value.ToEnum<CustomerState>());
            }
            if (model.AffiliateId.HasValue)
            {
                customerQuery = customerQuery.Where(x => x.IsAffiliate && x.AgentId == model.AffiliateId.Value);
            }

            if (model.SortType == SortType.Descending)
            {
                customerQuery = model.SortName == "state" ? customerQuery.OrderByDescending(x => x.State).ThenBy(x => x.Username) : customerQuery.OrderByDescending(GetSortCustomerProperty(model));
            }
            else
            {
                customerQuery = model.SortName == "state" ? customerQuery.OrderBy(x => x.State).ThenBy(x => x.Username) : customerQuery.OrderBy(GetSortCustomerProperty(model));
            }

            return new GetCustomerOfAgentManagerResult
            {
                Customers = await customerQuery
                            .Skip(model.PageSize * model.PageIndex)
                            .Take(model.PageSize).ToListAsync(),
                Metadata = new HnMicro.Framework.Responses.ApiResponseMetadata
                {
                    NoOfRows = await customerQuery.CountAsync(),
                    NoOfRowsPerPage = model.PageSize,
                    Page = model.PageIndex
                }
            };
        }

        private static Expression<Func<CustomerModel, object>> GetSortCustomerProperty(GetCustomerOfAgentManagerModel model)
        {
            if (string.IsNullOrEmpty(model.SortName)) return customer => customer.State;
            return model.SortName?.ToLower() switch
            {
                "username" => customer => customer.Username,
                "createddate" => customer => customer.CreatedDate,
                "state" => customer => customer.State,
                "fullname" => customer => customer.FirstName + " " + customer.LastName,
                "email" => customer => customer.Email,
                "telegram" => customer => customer.Telegram,
                "phone" => customer => customer.Phone,
                "rolename" => customer => customer.RoleName,
                "affiliateusername" => customer => customer.AffiliateUsername,
                _ => customer => customer
            };
        }
    }
}
