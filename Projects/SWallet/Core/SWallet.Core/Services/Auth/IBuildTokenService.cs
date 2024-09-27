using HnMicro.Core.Scopes;
using HnMicro.Framework.Models;
using SWallet.Core.Models.Customers;
using SWallet.Core.Models;

namespace SWallet.Core.Services.Auth
{
    public interface IBuildTokenService : IScopedDependency
    {
        JwtToken BuildToken(ManagerModel managerModel, ManagerSessionModel managerSessionModel);
        JwtToken BuildToken(CustomerModel customerModel, CustomerSessionModel customerSessionModel);
    }
}
