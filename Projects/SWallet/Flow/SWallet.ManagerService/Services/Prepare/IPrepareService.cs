using HnMicro.Core.Scopes;
using SWallet.ManagerService.Models.Prepare;

namespace SWallet.ManagerService.Services.Prepare
{
    public interface IPrepareService : IScopedDependency
    {
        Task<bool> InitialRoles();
        Task<CreateRootManagerResponseModel> CreateRootManager(CreateRootManagerModel createRootManagerModel);
    }
}
