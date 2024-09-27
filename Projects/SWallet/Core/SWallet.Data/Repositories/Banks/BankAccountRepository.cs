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

        public async Task<bool> CheckExistAccountNumber(int bankId, string accountNumber)
        {
            return await DbSet.AnyAsync(f => f.BankId == bankId && f.NumberAccount.ToLower() == accountNumber.ToLower());
        }

        public async Task<bool> CheckExistAccountNumberWhenUpdate(int bankId, string accountNumber, int bankAccountId)
        {
            return await DbSet.AnyAsync(f => f.BankId == bankId && f.NumberAccount.ToLower() == accountNumber.ToLower() && f.BankAccountId != bankAccountId);
        }

        public async Task<BankAccount> FindByBankAndBankAccount(int bankId, int bankAccountId)
        {
            return await DbSet.FirstOrDefaultAsync(f => f.BankId == bankId && f.BankAccountId == bankAccountId);
        }

        public async Task<List<BankAccount>> GetBankAccountByBankId(int bankId)
        {
            return await DbSet.Include(f => f.Bank).Where(f => f.BankId == bankId).OrderBy(f => f.NumberAccount).ThenBy(f => f.CardHolder).ToListAsync();
        }

        public async Task<List<BankAccount>> GetDepositBankAccountByBankId(int bankId)
        {
            return await DbSet.Where(f => f.BankId == bankId && f.DepositEnabled).ToListAsync();
        }
    }
}
