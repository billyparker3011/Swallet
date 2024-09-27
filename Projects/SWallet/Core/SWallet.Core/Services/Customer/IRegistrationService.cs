using HnMicro.Core.Scopes;
using HnMicro.Framework.Models;
using SWallet.Core.Models.Customers;

namespace SWallet.Core.Services.Customer
{
    public interface IRegistrationService : IScopedDependency
    {
        Task<JwtToken> Register(RegisterModel model);
    }
}
