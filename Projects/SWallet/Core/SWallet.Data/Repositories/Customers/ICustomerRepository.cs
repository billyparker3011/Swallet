using HnMicro.Modules.EntityFrameworkCore.Repositories;
using SWallet.Data.Core;
using SWallet.Data.Core.Entities;

namespace SWallet.Data.Repositories.Customers
{
    public interface ICustomerRepository : IEntityFrameworkCoreRepository<long, Customer, SWalletContext>
    {
        Task<Customer> FindByUsername(string username);
        Task<Customer> FindByUsernameAndPassword(string username, string password);
        Task<Customer> FindByEmail(string email);
    }
}
