using HnMicro.Core.Scopes;
using SWallet.Core.Models.Transactions;

namespace SWallet.Core.Services.Transaction
{
    public interface ITransactionService : IScopedDependency
    {
        Task<TransactionsHistoryResultModel> GetTransactionsHistory(GetTransactionsHistoryModel model);
    }
}
