using HnMicro.Modules.EntityFrameworkCore.Repositories;
using SWallet.Data.Core;
using SWallet.Data.Core.Entities;

namespace SWallet.Data.Repositories.Transactions
{
    public class TransactionRepository : EntityFrameworkCoreRepository<long, Transaction, SWalletContext>, ITransactionRepository
    {
        public TransactionRepository(SWalletContext context) : base(context)
        {
        }
    }
}
