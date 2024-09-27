using HnMicro.Core.Helpers;
using SWallet.Core.Enums;
using SWallet.Core.Models.Transactions;
using SWallet.Data.Core.Entities;

namespace SWallet.Core.Converters
{
    public static class TransactionConverter
    {
        public static TransactionModel ToTransactionModel(this Transaction transaction)
        {
            return new TransactionModel
            {
                TransactionId = transaction.TransactionId,
                TransactionName = string.IsNullOrEmpty(transaction.TransactionName) ? $"#{transaction.TransactionId}" : transaction.TransactionName,
                CustomerId = transaction.CustomerId,
                TransactionState = transaction.TransactionState.ToEnum<TransactionState>(),
                TransactionType = transaction.TransactionState.ToEnum<TransactionType>(),
                Amount = transaction.Amount,
                CreatedAt = transaction.CreatedAt,
                OriginAmount = transaction.OriginAmount
            };
        }
    }
}
