using HnMicro.Core.Scopes;
using SWallet.Core.Models;

namespace SWallet.Core.Services.Manager
{
    public interface IManagerService : IScopedDependency
    {
        Task CreateManager(CreateManagerModel model);
        Task<GetManagersResult> GetManagers(GetManagersModel model);
        Task<GetCustomerOfAgentManagerResult> GetCustomerOfAgentManager(GetCustomerOfAgentManagerModel model);
    }
}
