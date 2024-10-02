using HnMicro.Core.Scopes;
using SWallet.Core.Models.Customers;

namespace SWallet.Core.Services.Customer
{
    public interface ICustomerService : IScopedDependency
    {
        Task ChangeInfo(ChangeInfoModel model);
        Task<MyBalanceCustomerModel> MyBalance();
        Task<MyCustomerProfileModel> CustomerProfile(long customerId = 0L);
    }
}
