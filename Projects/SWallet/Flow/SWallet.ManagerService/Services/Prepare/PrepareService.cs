using HnMicro.Core.Helpers;
using HnMicro.Framework.Services;
using Microsoft.EntityFrameworkCore;
using SWallet.Core.Consts;
using SWallet.Core.Contexts;
using SWallet.Core.Enums;
using SWallet.Core.Helpers;
using SWallet.Core.Services;
using SWallet.Data.Repositories.Customers;
using SWallet.Data.Repositories.FeaturesAndPermissions;
using SWallet.Data.Repositories.Managers;
using SWallet.Data.Repositories.Payments;
using SWallet.Data.Repositories.Roles;
using SWallet.Data.Repositories.Settings;
using SWallet.Data.UnitOfWorks;
using SWallet.ManagerService.Models.Prepare;

namespace SWallet.ManagerService.Services.Prepare
{
    public class PrepareService : SWalletBaseService<PrepareService>, IPrepareService
    {
        public PrepareService(ILogger<PrepareService> logger, IServiceProvider serviceProvider, IConfiguration configuration, IClockService clockService, ISWalletClientContext clientContext, ISWalletUow sWalletUow) : base(logger, serviceProvider, configuration, clockService, clientContext, sWalletUow)
        {
        }

        public async Task<CreateRootManagerResponseModel> CreateRootManager(CreateRootManagerModel model)
        {
            ValidationPrepareToken();

            var roleRepository = SWalletUow.GetRepository<IRoleRepository>();
            var role = await roleRepository.GetRoleByRoleCode(RoleConsts.RoleAsRoot);
            if (role == null) return null;

            var managerRepository = SWalletUow.GetRepository<IManagerRepository>();
            var managerSessionRepository = SWalletUow.GetRepository<IManagerSessionRepository>();

            var anyRoot = await managerRepository.AnyRoot(ManagerRole.Root.ToInt(), role.RoleId);
            if (anyRoot) return null;

            var username = model.LengthOfUsername.RandomString().ToUpper();
            var password = model.LengthOfPassword.RandomString(true);

            var manager = new Data.Core.Entities.Manager
            {
                ManagerRole = ManagerRole.Root.ToInt(),
                Username = username,
                Password = password.Md5(),
                FullName = ManagerRole.Root.ToString(),
                State = ManagerState.Open.ToInt(),
                Role = role,
                CreatedAt = ClockService.GetUtcNow(),
                CreatedBy = 0L
            };

            var managerSession = new Data.Core.Entities.ManagerSession
            {
                State = SessionState.Initial.ToInt()
            };

            manager.ManagerSession = managerSession;
            managerSession.Manager = manager;

            managerSessionRepository.Add(managerSession);
            managerRepository.Add(manager);

            await SWalletUow.SaveChangesAsync();

            return new CreateRootManagerResponseModel
            {
                Username = username,
                Password = password,
                Fullname = manager.FullName
            };
        }

        public async Task<bool> InitialCustomerLevels()
        {
            ValidationPrepareToken();

            var items = Core.Helpers.EnumHelper.GetListCustomerLevelInfo();
            if (items.Count == 0) return false;

            var levelRepository = SWalletUow.GetRepository<ILevelRepository>();

            var currentLevels = await levelRepository.GetLevelByIds(items.Select(f => f.Value.ToInt()).ToList());
            if (currentLevels.Count > 0) return false;

            foreach (var item in items)
            {
                levelRepository.Add(new Data.Core.Entities.Level
                {
                    LevelId = item.Value.ToInt(),
                    LevelName = item.Code,
                    LevelCode = item.Code,
                    ShortDescription = item.Code,
                    FullDescription = item.Code,
                    CreatedAt = ClockService.GetUtcNow(),
                    CreatedBy = 0L
                });
            }
            return (await SWalletUow.SaveChangesAsync()) > 0;
        }

        public async Task<bool> InitialFeaturesAndPermissions()
        {
            ValidationPrepareToken();

            var allFeatureCode = FeatureAndPermissionHelper.AllFeatures.Select(f => f.FeatureCode).ToList();
            var allPermissionCode = FeatureAndPermissionHelper.AllFeatures.SelectMany(f => f.Permissions).Select(f => f.PermissionCode).ToList();

            var featureRepository = SWalletUow.GetRepository<IFeatureRepository>();
            var features = await featureRepository.FindQueryBy(f => allFeatureCode.Contains(f.FeatureName)).ToListAsync();

            var permissionRepository = SWalletUow.GetRepository<IPermissionRepository>();
            var permissions = await permissionRepository.FindQueryBy(f => allPermissionCode.Contains(f.PermissionCode)).ToListAsync();

            foreach (var item in FeatureAndPermissionHelper.AllFeatures)
            {
                var feature = features.FirstOrDefault(f => f.FeatureCode == item.FeatureCode);
                if (feature == null)
                {
                    feature = new Data.Core.Entities.Feature
                    {
                        FeatureName = item.FeatureName,
                        FeatureCode = item.FeatureCode,
                        CreatedAt = ClockService.GetUtcNow(),
                        CreatedBy = 0L,
                        Enabled = true
                    };
                    featureRepository.Add(feature);

                    foreach (var itemPermission in item.Permissions)
                    {
                        permissionRepository.Add(new Data.Core.Entities.Permission
                        {
                            Feature = feature,
                            CreatedAt = ClockService.GetUtcNow(),
                            CreatedBy = 0L,
                            PermissionCode = itemPermission.PermissionCode,
                            PermissionName = itemPermission.PermissionName
                        });
                    }
                }
                else
                {
                    foreach (var itemPermission in item.Permissions)
                    {
                        var permission = permissions.FirstOrDefault(f => f.PermissionCode == itemPermission.PermissionCode);
                        if (permission != null) continue;

                        permissionRepository.Add(new Data.Core.Entities.Permission
                        {
                            Feature = feature,
                            CreatedAt = ClockService.GetUtcNow(),
                            CreatedBy = 0L,
                            PermissionCode = itemPermission.PermissionCode,
                            PermissionName = itemPermission.PermissionName
                        });
                    }
                }
            }

            return (await SWalletUow.SaveChangesAsync()) > 0;
        }

        public async Task<bool> InitialManualPayment()
        {
            ValidationPrepareToken();

            var settingRepository = SWalletUow.GetRepository<ISettingRepository>();
            var actualSetting = await settingRepository.GetActualSetting();
            if (actualSetting == null || actualSetting.PaymentPartner != PaymentPartner.Manual.ToInt()) return false;

            var paymentMethodRepository = SWalletUow.GetRepository<IPaymentMethodRepository>();
            var paymentMethods = await paymentMethodRepository.FindByPaymentPartner(actualSetting.PaymentPartner);
            if (paymentMethods.Count != 0) return true;

            var ibPaymentMethod = paymentMethods.FirstOrDefault(f => f.Code == Core.InstanceOfPayment.Manual.Config.InternetBankingCode);
            if (ibPaymentMethod == null)
            {
                paymentMethodRepository.Add(new Data.Core.Entities.PaymentMethod
                {
                    Code = Core.InstanceOfPayment.Manual.Config.InternetBankingCode,
                    Name = Core.InstanceOfPayment.Manual.Config.InternetBankingName,
                    Icon = Core.InstanceOfPayment.Manual.Config.InternetBankingIcon,
                    Enabled = true,
                    Fee = 0m,
                    PaymentPartner = PaymentPartner.Manual.ToInt(),
                    CreatedAt = ClockService.GetUtcNow(),
                    CreatedBy = 0L
                });
            }

            return (await SWalletUow.SaveChangesAsync()) > 0;
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

        public async Task<bool> InitialSettings()
        {
            ValidationPrepareToken();

            var settingRepository = SWalletUow.GetRepository<ISettingRepository>();
            var actualSetting = await settingRepository.GetActualSetting();
            if (actualSetting == null)
            {
                actualSetting = new Data.Core.Entities.Setting
                {
                    CreatedAt = ClockService.GetUtcNow(),
                    CreatedBy = 0L,
                    CurrencySymbol = "USD",
                    MaskCharacter = "X",
                    NumberOfMaskCharacters = 4,
                    PaymentPartner = PaymentPartner.Manual.ToInt(),
                    DateTimeOffSet = 7,
                    MainDomain = "https://abc.com"
                };
                settingRepository.Add(actualSetting);
            }
            else
            {
                if (actualSetting.PaymentPartner == 0) actualSetting.PaymentPartner = PaymentPartner.Manual.ToInt();
                actualSetting.UpdatedAt = ClockService.GetUtcNow();
                actualSetting.UpdatedBy = 0L;
            }

            return (await SWalletUow.SaveChangesAsync()) > 0;
        }
    }
}
