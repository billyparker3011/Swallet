using HnMicro.Modules.EntityFrameworkCore.Repositories;
using SWallet.Data.Core;
using SWallet.Data.Core.Entities;

namespace SWallet.Data.Repositories.Transactions
{
    public interface ITransactionRepository : IEntityFrameworkCoreRepository<long, Transaction, SWalletContext>
    {
    }
}
