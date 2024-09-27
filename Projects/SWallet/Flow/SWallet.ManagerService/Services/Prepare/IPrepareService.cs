using HnMicro.Core.Scopes;
using SWallet.ManagerService.Models.Prepare;

namespace SWallet.ManagerService.Services.Prepare
{
    public interface IPrepareService : IScopedDependency
    {
        Task<bool> InitialRoles();
        Task<bool> InitialCustomerLevels();
        Task<bool> InitialFeaturesAndPermissions();
        Task<CreateRootManagerResponseModel> CreateRootManager(CreateRootManagerModel model);
        Task<bool> InitialSettings();
        Task<bool> InitialManualPayment();
    }
}
