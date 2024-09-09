using HnMicro.Modules.EntityFrameworkCore.Repositories;
using Microsoft.EntityFrameworkCore;
using SWallet.Data.Core;
using SWallet.Data.Core.Entities;

namespace SWallet.Data.Repositories.Banks
{
    public class BankRepository : EntityFrameworkCoreRepository<int, Bank, SWalletContext>, IBankRepository
    {
        public BankRepository(SWalletContext context) : base(context)
        {
        }

        public async Task<List<Bank>> GetDepositBanks()
        {
            return await DbSet.Where(f => f.DepositEnabled).OrderBy(f => f.Name).ToListAsync();
        }

        public async Task<List<Bank>> GetWithdrawBanks()
        {
            return await DbSet.Where(f => f.WithdrawEnabled).OrderBy(f => f.Name).ToListAsync();
        }
    }
}
