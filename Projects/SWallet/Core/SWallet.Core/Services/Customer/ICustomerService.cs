using HnMicro.Core.Scopes;
using SWallet.Core.Models.Customers;

namespace SWallet.Core.Services.Customer
{
    public interface ICustomerService : IScopedDependency
    {
        Task ChangeInfo(ChangeInfoModel model);
        Task<MyCustomerProfileModel> MyProfile(long? customerId);
    }
}
