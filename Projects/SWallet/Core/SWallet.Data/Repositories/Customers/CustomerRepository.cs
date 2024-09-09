using HnMicro.Modules.EntityFrameworkCore.Repositories;
using Microsoft.EntityFrameworkCore;
using SWallet.Data.Core;
using SWallet.Data.Core.Entities;

namespace SWallet.Data.Repositories.Customers
{
    public class CustomerRepository : EntityFrameworkCoreRepository<long, Customer, SWalletContext>, ICustomerRepository
    {
        public CustomerRepository(SWalletContext context) : base(context)
        {
        }

        public async Task<Customer> FindByEmail(string email)
        {
            return await DbSet.FirstOrDefaultAsync(f => f.Email.Equals(email, StringComparison.OrdinalIgnoreCase));
        }

        public async Task<Customer> FindByUsername(string username)
        {
            return await DbSet.FirstOrDefaultAsync(f => f.Username.Equals(username, StringComparison.OrdinalIgnoreCase));
        }

        public async Task<Customer> FindByUsernameAndPassword(string username, string password)
        {
            return await DbSet.FirstOrDefaultAsync(f => f.Username.Equals(username, StringComparison.OrdinalIgnoreCase) && f.Password.Equals(password, StringComparison.Ordinal));
        }
    }
}
