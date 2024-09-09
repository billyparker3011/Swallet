using HnMicro.Modules.EntityFrameworkCore.Repositories;
using SWallet.Data.Core;
using SWallet.Data.Core.Entities;

namespace SWallet.Data.Repositories.Banks
{
    public interface IBankRepository : IEntityFrameworkCoreRepository<int, Bank, SWalletContext>
    {
        Task<List<Bank>> GetDepositBanks();
        Task<List<Bank>> GetWithdrawBanks();
    }
}
