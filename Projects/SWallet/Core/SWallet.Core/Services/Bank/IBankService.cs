using HnMicro.Core.Scopes;
using SWallet.Core.Models;
using SWallet.Core.Models.Bank;
using SWallet.Core.Models.Bank.GetBanks;

namespace SWallet.Core.Services.Bank
{
    public interface IBankService : IScopedDependency
    {
        Task CreateBank(CreateBankModel model);
        Task<GetBanksResult> GetBanks(GetBanksModel query);
        Task UpdateBank(int id, CreateBankModel model);
        Task DeleteBank(int id);
        Task<List<BankModel>> GetBankBy(bool deposit, bool withdraw);
        Task<bool> CheckExistBank(string bankName, int? bankId);
    }
}
