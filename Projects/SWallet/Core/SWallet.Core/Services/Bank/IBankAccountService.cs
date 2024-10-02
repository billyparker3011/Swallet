using HnMicro.Core.Scopes;
using SWallet.Core.Models;
using SWallet.Core.Models.Bank.GetBankAccounts;

namespace SWallet.Core.Services.Bank
{
    public interface IBankAccountService : IScopedDependency 
    {
        Task CreateBankAccount(CreateBankAccountModel model);
        Task<GetBankAccountsResult> GetBankAccounts();
        Task UpdateBankAccount(int id, CreateBankAccountModel model);
        Task DeleteBankAccount(int id);
        Task<bool> CheckExistBankAccount(int bankId, string accountNumber, int? bankAccountId);
    }
}
