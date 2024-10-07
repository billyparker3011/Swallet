using HnMicro.Core.Scopes;
using SWallet.Core.Models.Transactions;

namespace SWallet.Core.Services.Transaction
{
    public interface ITransactionService : IScopedDependency
    {
        Task CompletedTransaction(CompletedTransactionModel model);
        Task<TransactionsHistoryResultModel> GetTransactions(GetTransactionsHistoryModel model);
        Task<TransactionsHistoryResultModel> GetTransactionsOfCustomer(GetTransactionsHistoryModel model);

        Task<TransactionsHistoryResultModel> GetTransactionsHistory(GetTransactionsHistoryModel model);
        Task<bool> RejectedTransaction(long transactionId);
    }
}
