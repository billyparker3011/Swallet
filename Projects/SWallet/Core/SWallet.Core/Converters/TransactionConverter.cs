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
            DiscountTransactionModel discountModel = null;
            if (transaction.TransactionType == TransactionType.Discount.ToInt())
            {
                discountModel = new DiscountTransactionModel
                {
                    DiscountId = transaction.DiscountId.Value,
                    ReferenceDiscountDetail = transaction.ReferenceDiscountDetail.Value,
                    ReferenceTransactionId = transaction.ReferenceTransactionId.Value
                };
            }
            DepositTransactionModel depositModel = null;
            if (transaction.TransactionType == TransactionType.Deposit.ToInt())
            {
                depositModel = new DepositTransactionModel
                {
                    DepositBankName = transaction.DepositBankName,
                    DepositCardHolder = transaction.DepositCardHolder,
                    DepositContent = transaction.DepositContent,
                    DepositNumberAccount = transaction.DepositNumberAccount,
                    DepositToBankName = transaction.DepositToBankName,
                    DepositToCardHolder = transaction.DepositToCardHolder,
                    DepositToNumberAccount = transaction.DepositToNumberAccount
                };
            }
            WithdrawTransactionModel withdrawModel = null;
            if (transaction.TransactionType == TransactionType.Withdraw.ToInt())
            {
                withdrawModel = new WithdrawTransactionModel
                {
                    WithdrawBankName = transaction.WithdrawBankName,
                    WithdrawCardHolder = transaction.WithdrawCardHolder,
                    WithdrawNumberAccount = transaction.WithdrawNumberAccount,
                    WithdrawToBankName = transaction.WithdrawToBankName,
                    WithdrawToCardHolder = transaction.WithdrawToCardHolder,
                    WithdrawToNumberAccount = transaction.WithdrawToNumberAccount
                };
            }
            return new TransactionModel
            {
                TransactionId = transaction.TransactionId,
                TransactionName = string.IsNullOrEmpty(transaction.TransactionName) ? $"#{transaction.TransactionId}" : transaction.TransactionName,
                CustomerId = transaction.CustomerId,
                CustomerUsername = transaction.Customer != null ? transaction.Customer.Username : string.Empty,
                TransactionState = transaction.TransactionState.ToEnum<TransactionState>(),
                TransactionType = transaction.TransactionType.ToEnum<TransactionType>(),
                Amount = transaction.Amount,
                CreatedAt = transaction.CreatedAt,
                OriginAmount = transaction.OriginAmount,
                Discount = discountModel,
                Deposit = depositModel,
                Withdraw = withdrawModel
            };
        }
    }
}
