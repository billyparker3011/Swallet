using HnMicro.Core.Scopes;
using SWallet.Core.Models.Auth;

namespace SWallet.Core.Services.Auth
{
    public interface IPasswordService : IScopedDependency
    {
        Task ChangeCustomerPassword(ChangePasswordModel model);
    }
}
