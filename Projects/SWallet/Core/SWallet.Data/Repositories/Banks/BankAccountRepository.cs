using HnMicro.Modules.EntityFrameworkCore.Repositories;
using Microsoft.EntityFrameworkCore;
using SWallet.Data.Core;
using SWallet.Data.Core.Entities;

namespace SWallet.Data.Repositories.Banks
{
    public class BankAccountRepository : EntityFrameworkCoreRepository<int, BankAccount, SWalletContext>, IBankAccountRepository
    {
        public BankAccountRepository(SWalletContext context) : base(context)
        {
        }

        public async Task<List<BankAccount>> GetBankAccountByBankId(int bankId)
        {
            return await DbSet.Include(f => f.Bank).Where(f => f.BankId == bankId).OrderBy(f => f.NumberAccount).ThenBy(f => f.CardHolder).ToListAsync();
        }
    }
}
