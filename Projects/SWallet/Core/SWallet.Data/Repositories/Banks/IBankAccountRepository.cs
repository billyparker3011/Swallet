using HnMicro.Modules.EntityFrameworkCore.Repositories;
using SWallet.Data.Core;
using SWallet.Data.Core.Entities;

namespace SWallet.Data.Repositories.Banks
{
    public interface IBankAccountRepository : IEntityFrameworkCoreRepository<int, BankAccount, SWalletContext>
    {
        Task<List<BankAccount>> GetBankAccountByBankId(int bankId);
    }
}
