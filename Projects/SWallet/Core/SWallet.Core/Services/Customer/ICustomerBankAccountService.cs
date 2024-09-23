using HnMicro.Core.Scopes;
using SWallet.Core.Models;

namespace SWallet.Core.Services.Customer
{
    public interface ICustomerBankAccountService : IScopedDependency
    {
        Task AddOrUpdate(AddOrUpdateCustomerBankAccountModel model, long id = 0L);
        Task<List<CustomerBankAccountModel>> GetCurrentCustomerBankAccounts();
        Task<List<CustomerBankAccountModel>> GetCustomerBankAccounts(long customerId);
    }
}
