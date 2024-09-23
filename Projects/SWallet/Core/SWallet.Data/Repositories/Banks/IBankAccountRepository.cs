using HnMicro.Modules.EntityFrameworkCore.Repositories;
using SWallet.Data.Core;
using SWallet.Data.Core.Entities;

namespace SWallet.Data.Repositories.Banks
{
    public interface IBankAccountRepository : IEntityFrameworkCoreRepository<int, BankAccount, SWalletContext>
    {
        Task<List<BankAccount>> GetBankAccountByBankId(int bankId);
        Task<bool> CheckExistAccountNumber(int bankId, string accountNumber);
        Task<bool> CheckExistAccountNumberWhenUpdate(int bankId, string accountNumber, int bankAccountId);
        Task<List<BankAccount>> GetDepositBankAccountByBankId(int bankId);
        Task<BankAccount> FindByBankAndBankAccount(int bankId, int bankAccountId);
    }
}
